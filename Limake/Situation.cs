using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public class Situation : IComparer<Position>
    {
        private static readonly int[] GoalLimits = { 0, 28, 7, 14, 21 };
        private static readonly Position[] GoalStarts = { Position.None, Position.GreenGoal1, Position.RedGoal1, Position.BlueGoal1, Position.YellowGoal1 };
        private static readonly Position[] HomeStarts = { Position.None, Position.GreenHome1, Position.RedHome1, Position.BlueHome1, Position.YellowHome1 };
        private static readonly Position[] HomeEnds = { Position.None, Position.GreenHome4, Position.RedHome4, Position.BlueHome4, Position.YellowHome4 };
        private static readonly Position[] SafetySpots = { Position.None, Position.GreenStart, Position.RedStart, Position.BlueStart, Position.YellowStart };
        private const int GoalStart = (int)Position.GreenGoal1;
        private const int GoalEnd = (int)Position.YellowGoal4;

        // 28 board spaces + 16 hjembo + 16 goal
        public Piece[] board;
        // 16 pieces
        public Position[] pieces;

        public List<Animation>[] animations;

        // Amout of beers for each player to drink
        public int[] beers;

        public Dictionary<int, List<int>> groupingDict;

        public Situation()
        {
            this.board = new Piece[60];
            this.pieces = new Position[16];
            this.beers = new int[5];
            this.animations = new List<Animation>[2];
            for (int i = 0; i < 5; i++)
            {
                this.beers[i] = 0;
            }
            this.groupingDict = new Dictionary<int, List<int>>();
        }

        public Situation(Situation s)
        {
            this.board = (Piece[])s.board.Clone();
            this.pieces = (Position[])s.pieces.Clone();
            this.beers = (int[])s.beers.Clone();
            this.animations = new List<Animation>[2];
            this.groupingDict = new Dictionary<int, List<int>>(s.groupingDict);
        }

        public Situation(Position[] pieces) : this()
        {
            this.pieces = pieces;
            int currentPiece = 1;
            for (int i = 0; i < pieces.Length; i++)
            {
                if (i % 4 == 0 && i > 0)
                {
                    currentPiece++;
                }
                this.board[(int)pieces[i]] = (Piece)currentPiece;
            }
            this.GenerateGroupings();
        }

        public Move[] GetMoves(Piece side, int Roll)
        {
            int start = ((byte)side - 1) * 4;
            int end = start + 4;

            List<Move> moves = new List<Move>();
            List<int> skipPieces = null;

            for (int i = start; i < end; i++)
            {
                Position pos = pieces[i];
                if (skipPieces == null)
                {
                    if (this.groupingDict.ContainsKey(i))
                    {
                        skipPieces = new List<int> (this.groupingDict[i]);
                    }
                }
                else
                {
                    if (skipPieces.Contains(i))
                    {
                        continue;
                    }
                    if (this.groupingDict.ContainsKey(i))
                    {
                        skipPieces.AddRange(this.groupingDict[i]);
                    }
                }
                byte bpos = (byte)pos;
                int newPos = 0;

                // Hjembo
                if (bpos >= 44)
                {
                    if (Roll == 6)
                    {
                        Position endPosition;
                        switch (side)
                        {
                            case Piece.Green: endPosition = Position.GreenStart; break;
                            case Piece.Red: endPosition = Position.RedStart; break;
                            case Piece.Blue: endPosition = Position.BlueStart; break;
                            case Piece.Yellow: endPosition = Position.YellowStart; break;
                            default: throw new Exception();
                        }
                        MoveType type = MoveType.OutOfHome;
                        if (board[(int)endPosition] == side)
                        {
                            type = MoveType.DoubleUp;
                        }
                        moves.Add(new Move(i, pos, endPosition, type));
                    }
                    //Hjembo is the last, skip rest
                    break;
                }
                // Inside goal
                else if (bpos >= 28)
                {
                    newPos = bpos + Roll;
                    if (newPos >= (int)GoalStarts[(int)side] + 4)
                    {
                        // Beyond limits of the goal
                        continue;
                    }
                }
                else
                {
                    newPos = bpos + Roll;
                    if (bpos < GoalLimits[(int)side] && newPos >= GoalLimits[(int)side])
                    {
                        int intoGoal = newPos - GoalLimits[(int)side];
                        if (intoGoal < 4)
                        {
                            newPos = (int)GoalStarts[(int)side] + intoGoal;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        newPos = newPos % 28;
                    }
                }

                // It's not possible to go into a position that you already have a piece in
                Piece targetPiece = board[newPos];
                if (targetPiece != side)
                {
                    if ((Position)newPos == SafetySpots[(int)targetPiece])
                    {
                        int endPos = GetFreeHomePosition(side);
                        moves.Add(new Move(i, pos, (Position)(endPos), MoveType.SelfTackle, (Position)newPos));
                    }
                    else
                    {
                        MoveType t;
                        if (targetPiece == Piece.None)
                        {
                            t = MoveType.Move;
                        }
                        else
                        {
                            t = MoveType.Eat;
                        }
                        moves.Add(new Move(i, pos, (Position)(newPos), t));
                    }
                }
            }

            return moves.ToArray();
        }

        public void ApplyMove(Move move)
        {
            this.ClearAnimations();

            Piece endPiece = board[(int)move.EndPosition];
            if (endPiece != Piece.None && move.Type != MoveType.DoubleUp)
            {
                int currentPieceSize = 1;
                if (this.groupingDict.ContainsKey(move.Piece))
                {
                    currentPieceSize = this.groupingDict[(int)move.Piece].Count;
                }

                int piecePos = 0;
                while (pieces[piecePos] != move.EndPosition)
                {
                    piecePos++;
                }

                if (this.groupingDict.ContainsKey(piecePos))
                {
                    List<int> list = this.groupingDict[piecePos];
                    foreach (int i in list)
                    {
                        this.beers[(int)endPiece] += currentPieceSize;
                        MoveToHome(endPiece, i);
                    }
                }
                else
                {
                    this.beers[(int)endPiece] += currentPieceSize;
                    MoveToHome(endPiece, piecePos);
                }
                SortHome(endPiece);
            }
            if (this.groupingDict.ContainsKey(move.Piece))
            {
                List<int> groupPieces = this.groupingDict[move.Piece];
                
                if (move.Type == MoveType.SelfTackle)
                {
                    List<int> list = this.groupingDict[move.Piece];
                    foreach (int i in list)
                    {
                        AddAnimation(0, i, move.StartPosition, move.MiddlePosition);
                        MoveToHome(board[(int)move.StartPosition], i);
                    }
                }
                else
                {
                    foreach (int i in groupPieces)
                    {
                        pieces[i] = move.EndPosition;
                        this.AddAnimation(0, i, move.StartPosition, move.EndPosition);
                    }
                }
            }
            else
            {
                if (move.Type == MoveType.SelfTackle)
                {
                    AddAnimation(0, move.Piece, move.StartPosition, move.MiddlePosition);
                    AddAnimation(1, move.Piece, move.MiddlePosition, move.EndPosition);
                }
                else
                {
                    this.AddAnimation(0, move.Piece, move.StartPosition, move.EndPosition);
                }

                pieces[move.Piece] = move.EndPosition;
            }
            board[(int)move.EndPosition] = board[(int)move.StartPosition];
            board[(int)move.StartPosition] = Piece.None;

            Piece side = board[(int)move.EndPosition];
            int sideGoalStart = (int)GoalStarts[(int)side];
            int sideGroupingStart = ((int)side - 1) * 4;

            for (int i = 3; i > 0; i--)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (groupingDict.ContainsKey(sideGroupingStart + j))
                    {
                        List<int> g = groupingDict[sideGroupingStart + j];
                        if ((int)pieces[g[0]] == sideGoalStart+i && g.Count > 1)
                        {
                            int last = g.Count - 1;
                            for (int k = 0; k < i && last > 0; k++)
                            {
                                int pos = sideGoalStart + k;
                                if (board[pos] == Piece.None)
                                {
                                    pieces[g[last]] = (Position)pos;
                                    board[pos] = side;
                                    AddAnimation(1, g[last], pieces[g[0]], (Position)pos); 
                                    last--;
                                }
                            }
                        }
                    }
                }

            }

            this.GenerateGroupings();
        }

        private void MoveToHome(Piece side, int piecePos)
        {
            Console.WriteLine("MoveToHome: " + piecePos + " " + side);
            int homePos = GetFreeHomePosition(side);
            Console.WriteLine("HomePos: " + (Position)homePos);
            board[homePos] = side;

            Console.WriteLine("Save: " + piecePos + " " + pieces[piecePos] + " " + (Position)homePos);
            AddAnimation(1, piecePos, pieces[piecePos], (Position)homePos);
            pieces[piecePos] = (Position)homePos;
        }

        private void SortHome(Piece side)
        {
            int start = ((int)side-1)*4;
            Console.WriteLine("SortHome: " + start);
            for (int i = 0; i < 16; i++)
            {
                Console.WriteLine(pieces[i]);
            }
            Array.Sort<Position>(pieces, start, 4, this);
            Console.WriteLine("After sorting");
            for (int i = 0; i < 16; i++)
            {
                Console.WriteLine(pieces[i]);
            }
        }

        private int GetFreeHomePosition(Piece endPiece)
        {
            int homePos = (int)HomeEnds[(int)endPiece];
            while (board[homePos] != Piece.None)
            {
                homePos--;
            }
            return homePos;
        }

        private bool IsGoalPosition(Position position)
        {
            int pos = (int)position;
            return pos >= GoalStart && pos <= GoalEnd;
        }

        private String p(Position pos)
        {
            Piece piece = board[(int)pos];
            switch (piece)
            {
                case Piece.Green: return "G";
                case Piece.Blue: return "B";
                case Piece.Red: return "R";
                case Piece.Yellow: return "Y";
                default: return "X";
            }

        }

        private String w(int length)
        {
            return "".PadLeft(length);
        }

        public override String ToString()
        {
            /*
               X               X
              X                 X
             X  X X X X X X X X  X
            X   Xoo         o X   X
                X  oo     oo  X  
                X        o    X  
                X    o        X  
                X  oo     oo  X  
            X   X o         ooX   X
             X  X X X X X X X X  X
              X                 X
               X               X  
             */
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("   {0}               {1}\n", p(Position.GreenHome1), p(Position.RedHome4));
            sb.AppendFormat("  {0}                 {1}\n", p(Position.GreenHome2), p(Position.RedHome3));

            sb.AppendFormat(" {0}  ", p(Position.GreenHome3));
            for(int ipos=0; ipos <= 7; ipos++) {
                sb.Append(p((Position)ipos)).Append(" ");
            }
            sb.AppendFormat(" {0}\n", p(Position.RedHome2));

            sb.AppendFormat("{0}   {1}{2}{3}         {4} {5}   {6}\n", p(Position.GreenHome4), p(Position.Yellow6), p(Position.GreenGoal1/**/), p(Position.GreenGoal2/**/), p(Position.RedGoal1), p(Position.Red1), p(Position.RedHome1));
            sb.AppendFormat("    {0}  {1}{2}     {3}{4}  {5}\n", p(Position.Yellow5), p(Position.GreenGoal3/**/), p(Position.GreenGoal4/**/), p(Position.RedGoal3), p(Position.RedGoal2), p(Position.Red2));
            sb.AppendFormat("    {0}        {1}    {2}\n", p(Position.Yellow4), p(Position.RedGoal4), p(Position.Red3));
            sb.AppendFormat("    {0}    {1}        {2}\n", p(Position.Yellow3), p(Position.YellowGoal4), p(Position.Red4));
            sb.AppendFormat("    {0}  {1}{2}     {3}{4}  {5}\n", p(Position.Yellow2), p(Position.YellowGoal2), p(Position.YellowGoal3), p(Position.BlueGoal4), p(Position.BlueGoal3), p(Position.Red5));
            sb.AppendFormat("{0}   {1} {2}         {3}{4}{5}   {6}\n", p(Position.YellowHome1), p(Position.Yellow1), p(Position.YellowGoal1), p(Position.BlueGoal2), p(Position.BlueGoal1), p(Position.Red6), p(Position.BlueHome4));

            sb.AppendFormat(" {0}  ", p(Position.YellowHome2));
            for (int i = 7; i >= 0; i--)
            {
                sb.Append(p((Position)((byte)Position.BlueStart + i))).Append(" ");
            }
            sb.AppendFormat(" {0}\n", p(Position.BlueHome3));

            sb.AppendFormat("  {0}                 {1}\n", p(Position.YellowHome3), p(Position.BlueHome2));
            sb.AppendFormat("   {0}               {1}\n", p(Position.YellowHome4), p(Position.BlueHome1));

            return sb.ToString();
        }

        public static Situation GameStart()
        {
            Situation s = new Situation();
            for (int i = 0; i < 16; i++)
            {
                int pos = (int)Position.GreenHome1 + i;
                s.board[pos] = (Piece)((i / 4)+1);
                s.pieces[i] = (Position)pos;
            }

            return s;
        }

        public Piece GetWinner()
        {
            for (int i = 0; i < 4; i++)
            {
                bool winner = true;
                int sideGoalStart = (byte)Position.GreenGoal1 + (4 * i);
                Piece currentSide = (Piece)(i+1);

                //Can't win while there's beer left
                if (this.beers[i + 1] > 0)
                    continue;

                for (int j = 0; j < 4; j++)
                {
                    if (this.board[sideGoalStart + j] != currentSide)
                    {
                        winner = false;
                        break;
                    }
                }
                if (winner)
                {
                    return currentSide;
                }
            }
            return Piece.None;
        }

        public int GetNumberOfTries(Piece side)
        {
            int homeEnd = (int)Position.GreenHome4 + ((int)side - 1) * 4;
            int goalEnd = (int)Position.GreenGoal4 + ((int)side - 1) * 4;
            int inGoal = 4, atHome = 4;
            for (int i = 0; i < 4; i++)
            {
                if (board[homeEnd - i] != side)
                {
                    atHome = i;
                    break;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (board[goalEnd - i] != side)
                {
                    inGoal = i;
                    break;
                }
            }

            if (inGoal == 4)
                return 0;
            if (atHome + inGoal == 4)
                return 3;
            return 1;
        }

        public int GetPieceCount(Position pos)
        {
            if (this.board[(int)pos] == Piece.None)
                return 0;
            int count = 0;
            for (int i = 0; i < 16; i++)
            {
                if (pieces[i] == pos)
                {
                    count++;
                }
            }
            return count;
        }

        private String errors;
        public String Errors
        {
            get { return errors; }
        }
        public bool Validate()
        {
            bool success = true;
            Piece[] compareboard = new Piece[(int)Position.None];
            for (int i = 0; i < 16; i++)
            {
                if (this.board[(int)this.pieces[i]] != (Piece)(1 + i / 4))
                {
                    errors += this.pieces[i] + " should be " + (Piece)(1 + i / 4) + " but was " + this.board[(int)this.pieces[i]];
                    success = false;
                }
                    
                compareboard[(int)this.pieces[i]] = this.board[(int)this.pieces[i]];
            }
            for (int i = 0; i < (int)Position.None; i++)
            {
                if (this.board[i] != compareboard[i])
                {
                    errors += (Position)i + " board is " + this.board[i] + " but according to pieces " + compareboard[i];
                    success = false;
                }
            }
            foreach(List<int> groupedPieces in this.groupingDict.Values) {
                for (int i = 0; i < groupedPieces.Count; i++)
                {
                    if (this.groupingDict[groupedPieces[i]].Count != groupedPieces.Count)
                    {
                        errors += "Error in grouping for " + groupedPieces[i];
                        success = false;
                    }
                }
            }
            return success;
        }

        private void GenerateGroupings()
        {
            this.groupingDict.Clear();
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (i == j) continue;

                    if (pieces[i] == pieces[j])
                    {
                        if (this.groupingDict.ContainsKey(i))
                        {
                            this.groupingDict[i].Add(j);
                        }
                        else
                        {
                            var list = new List<int>(4);
                            list.Add(i);
                            list.Add(j);
                            this.groupingDict[i] = list;
                        }
                    }
                }
            }
        }

        int IComparer<Position>.Compare(Position x, Position y)
        {
            int p1 = (int)x, p2 = (int)y;
            return p1-p2;
        }

        public void DrinkBeers(Piece side, int beers)
        {
            if(beers < 0)
                throw new ArgumentException("Can't drink negative beers");

            this.beers[(int)side] -= beers;
            if(this.beers[(int)side] < 0)
                this.beers[(int)side] = 0;
        }

        public void ClearAnimations()
        {
            this.animations[0] = new List<Animation>();
            this.animations[1] = new List<Animation>();
        }

        private void AddAnimation(int step, int piece, Position start, Position end)
        {
            this.animations[step].Add(new Animation() { Start = start, End = end, Piece = piece });
        }

        public bool HasMultistepAnimation()
        {
            return this.animations[1] != null && this.animations[1].Count > 0;
        }

        public struct Animation
        {
            public int Piece;
            public Position Start;
            public Position End;
        }
    }
}

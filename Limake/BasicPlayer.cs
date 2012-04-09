using System;
using System.Net;

namespace Limake
{
    public class BasicPlayer: IPlayer
    {
        Random rand = new Random();

        public BasicPlayer()
        {
        }

        public int SelectMove(Situation situation, Move[] moves, Piece side, int roll)
        {
            Move[] c = new Move[moves.Length];
            Array.Copy(moves, c, moves.Length);
            Array.Sort<Move>(c, CompareMoves);
            return Array.IndexOf<Move>(moves, c[0]);
        }

        private int CompareMoves(Move left, Move right)
        {
            // Maaliin jos pääsee
            if (IsInGoal(left.EndPosition) && !IsInGoal(left.StartPosition))
            {
                if (!IsInGoal(right.EndPosition) || IsInGoal(right.StartPosition))
                {
                    return -1;
                }
            }
            else if (IsInGoal(right.EndPosition) && !IsInGoal(left.StartPosition))
            {
                return 1;
            }

            // Syö, poistu kotipesästä, tuplaa aina kuin voit
            if (left.Type != right.Type)
            {
                MoveType[] preferences = { MoveType.Eat, MoveType.OutOfHome, MoveType.DoubleUp, MoveType.Move, MoveType.SelfTackle };
                foreach(MoveType preference in preferences) 
                {
                    if (left.Type == preference) return -1;
                    if (right.Type == preference) return 1;
                }
            }

            return 0;
        }

        private bool IsInGoal(Position pos)
        {
            return (int)pos >= (int)Position.GreenGoal1 && (int)pos <= (int)Position.YellowGoal4;
        }

        public void NoMovesAvailable(Situation situation)
        {
            // Nothing
        }

        public void AskForRoll()
        {
        }

        private int beerCounter = 0;
        public int HowManyBeersAreDrunk(Piece side)
        {
            if (++beerCounter % 40 == 0)
                return 1;
            return 0;
        }


        public void WaitForRoll()
        {
            // Nothing
        }
    }
}

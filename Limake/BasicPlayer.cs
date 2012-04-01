using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
            Move[] originalMoves = new Move[moves.Length];
            Array.Copy(moves, originalMoves, moves.Length);
            Array.Sort<Move>(moves, CompareMoves);
            return Array.IndexOf<Move>(originalMoves, moves[0]);
        }

        private int CompareMoves(Move left, Move right)
        {
            // Maaliin jos pääsee
            if (IsInGoal(left.EndPosition) && !IsInGoal(left.StartPosition))
            {
                if (!IsInGoal(right.EndPosition) || IsInGoal(right.StartPosition))
                {
                    return 1;
                }
            }
            else if (IsInGoal(right.EndPosition) && !IsInGoal(left.StartPosition))
            {
                return -1;
            }

            // Syö, poistu kotipesästä, tuplaa aina kuin voit
            if (left.Type != right.Type)
            {
                MoveType[] preferences = { MoveType.Eat, MoveType.OutOfHome, MoveType.DoubleUp, MoveType.Move, MoveType.SelfTackle };
                foreach(MoveType preference in preferences) 
                {
                    if (left.Type == preference) return 1;
                    if (right.Type == preference) return -1;
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
    }
}

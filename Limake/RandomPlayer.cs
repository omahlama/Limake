using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public class RandomPlayer : IPlayer
    {
        Random rand = new Random();

        public RandomPlayer()
        {
        }

        public int SelectMove(Situation situation, Move[] moves, Piece side, int roll)
        {
            return rand.Next(moves.Length);
        }

        public void NoMovesAvailable(Situation situation)
        {
            // Nothing
        }

        public void AskForRoll()
        {
        }

        public int HowManyBeersAreDrunk(Piece side)
        {
            int r = rand.Next(10);
            if (r > 8)
                return 1;
            return 0;
        }
    }
}

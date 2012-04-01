using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public class ConsoleGameDisplay : IGameDisplay
    {
        public ConsoleGameDisplay() { }

        void IGameDisplay.DisplayTurn(Piece side, int turn)
        {
            Console.WriteLine("Turn " + turn + " for side " + side);
        }

        void IGameDisplay.DisplayRoll(int Roll)
        {
            Console.WriteLine("Roll " + Roll);
        }

        void IGameDisplay.DisplaySituation(Situation situation)
        {
            Console.WriteLine(situation.ToString());
        }

        void IGameDisplay.DispĺayMove(Move move)
        {
            Console.WriteLine(move.ToString());
        }

        void IGameDisplay.DisplayNoMove()
        {
            Console.WriteLine("No move available");
        }
    }

}

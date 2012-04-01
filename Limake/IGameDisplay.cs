using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public interface IGameDisplay
    {
        void DisplayTurn(Piece side, int turn);
        void DisplayRoll(int Roll);
        void DisplaySituation(Situation situation);
        void DispĺayMove(Move move);
        void DisplayNoMove();
    }
}

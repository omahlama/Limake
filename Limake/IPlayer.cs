using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public interface IPlayer
    {
        int SelectMove(Situation situation, Move[] moves, Piece side, int roll);
        void NoMovesAvailable(Situation situation);
        int HowManyBeersAreDrunk(Piece side);
    }
}

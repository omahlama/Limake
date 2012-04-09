using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public struct Move
    {
        public int Piece;
        public Position StartPosition;
        public Position EndPosition;
        public MoveType Type;
        public Position MiddlePosition;

        public Move(int Piece, Position StartPosition, Position EndPosition, MoveType Type, Position MiddlePosition = Position.None)
        {
            this.Piece = Piece;
            this.StartPosition = StartPosition;
            this.EndPosition = EndPosition;
            this.MiddlePosition = MiddlePosition;
            this.Type = Type;
        }

        public override String ToString()
        {
            return "[" + Type + "] " + StartPosition + " --> " + EndPosition;
        }
    }
}

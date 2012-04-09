using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Limake;

namespace LimakeTest
{
    [TestFixture]
    public class BasicPlayerTest
    {
        private BasicPlayer p;

        private static Position[] getStartPositions()
        {
            return new Position[] { Position.GreenHome1, Position.GreenHome2, Position.GreenHome3, Position.GreenHome4,
                                    Position.RedHome1, Position.RedHome2,Position.RedHome3,Position.RedHome4,
                                    Position.BlueHome1,Position.BlueHome2,Position.BlueHome3,Position.BlueHome4,
                                    Position.YellowHome1,Position.YellowHome2,Position.YellowHome3,Position.YellowHome4};
        }

        [SetUp]
        public void Setup()
        {
            p = new BasicPlayer();
        }

        [Test]
        public void EatIfYouCan()
        {
            var pos = getStartPositions();
            pos[0] = Position.Blue1;
            pos[1] = Position.Green1;
            pos[4] = Position.Green3;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 2);

            int selected = p.SelectMove(sit, moves, Piece.Green, 2);
            var selectedMove = moves[selected];
            Assert.AreEqual(Position.Green1, selectedMove.StartPosition);
        }

        [Test]
        public void GetOutOfHomeIfPossible()
        {
            var pos = getStartPositions();
            pos[0] = Position.Blue2;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 6);

            int selected = p.SelectMove(sit, moves, Piece.Green, 6);
            var selectedMove = moves[selected];

            Assert.AreEqual(Position.GreenStart, selectedMove.EndPosition);
        }

        [Test]
        public void MoveToGoalIfPossible()
        {
            var pos = getStartPositions();
            pos[0] = Position.Yellow6;
            pos[1] = Position.Green1;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 2);

            int selected = p.SelectMove(sit, moves, Piece.Green, 2);
            var selectedMove = moves[selected];
            Assert.AreEqual(Position.Yellow6, selectedMove.StartPosition);
        }
    }
}

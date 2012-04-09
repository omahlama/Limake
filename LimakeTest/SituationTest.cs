using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Limake;

namespace LimakeTest
{
    [TestFixture]
    public class SituationTest
    {
        [Test]
        public void TestCreation()
        {
            Situation s = new Situation();
            Assert.IsNotNull(s);
        }

        [Test]
        public void TestStartPosition()
        {
            Situation s = Situation.GameStart();
            Assert.IsNotNull(s);

            // Vi bor i hjembo
            Assert.AreEqual(Piece.Green, s.board[(int)Position.GreenHome1]);
            Assert.AreEqual(Piece.Red, s.board[(int)Position.RedHome2]);
            Assert.AreEqual(Piece.Blue, s.board[(int)Position.BlueHome3]);
            Assert.AreEqual(Piece.Yellow, s.board[(int)Position.YellowHome4]);

            // Eikä muualla
            Assert.AreEqual(Piece.None, s.board[(int)Position.BlueStart]);
            Assert.AreEqual(Piece.None, s.board[(int)Position.Green3]);
            Assert.AreEqual(Piece.None, s.board[(int)Position.RedGoal1]);
        }

        [Test]
        public void TestNoMovesForFiveAtStart()
        {
            Situation s = Situation.GameStart();

            Move[] moves = s.GetMoves(Piece.Green, 5);
            Assert.AreEqual(0, moves.Length, "Five should not let you out of homebase");
        }

        [Test]
        public void TestSixGetsYouOutOfHome()
        {
            Situation s = Situation.GameStart();

            Move[] moves = s.GetMoves(Piece.Green, 6);
            Assert.IsNotNull(moves);
            Assert.AreEqual(1, moves.Length, "Should have one move, out of home base");
            if (moves.Length > 0)
            {
                Move move = moves[0];
                Assert.AreEqual(Position.GreenHome1, move.StartPosition, "Should start from first home");
                Assert.AreEqual(Position.GreenStart, move.EndPosition, "Should end in start position");
            }
            else
            {
                Assert.Fail("No moves, can't test first move");
            }
        }

        [Test]
        public void TestFirstMove()
        {
            Situation s = Situation.GameStart();

            Move[] moves = s.GetMoves(Piece.Blue, 6);
            s.ApplyMove(moves[0]);

            Assert.AreEqual(Piece.None, s.board[(int)Position.BlueHome1], "Home base should have one empty");
            Assert.AreEqual(Piece.Blue, s.board[(int)Position.BlueStart], "Start position should have a blue piece");
            Assert.AreEqual(Piece.Blue, s.board[(int)Position.BlueHome2], "Home base should have others full");
        }

        [Test]
        public void TestCloning()
        {
            Situation s = Situation.GameStart();
            s.ApplyMove(new Move(0, s.pieces[0], Position.Green4, MoveType.Move));
            Situation clone = new Situation(s);

            Assert.AreEqual(Piece.Green, clone.board[(int)Position.Green4], "Move should be ok on the clone");

            s.ApplyMove(new Move(0, s.pieces[0], Position.Green6, MoveType.Move));

            Assert.AreEqual(Piece.None, s.board[(int)Position.Green4], "Move should be have happened on the original");
            Assert.AreEqual(Piece.Green, s.board[(int)Position.Green6], "Move should be have happened on the original");
            Assert.AreEqual(Piece.Green, clone.board[(int)Position.Green4], "Clone should not be affected");
            Assert.AreEqual(Piece.None, clone.board[(int)Position.Green6], "Clone should not be affected");

            clone.ApplyMove(new Move(1, clone.pieces[1], Position.GreenGoal2, MoveType.Move));

            Assert.AreEqual(Position.GreenGoal2, clone.pieces[1], "Move should be have happened on the clone");
            Assert.AreEqual(Position.GreenHome2, s.pieces[1], "Move should be have happened on the clone");
        }

        [Test]
        public void TestBasicMove()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.GreenStart;
            Situation start = new Situation(pos);
            Move[] moves;
            for (int i = 1; i < 6; i++)
            {
                moves = start.GetMoves(Piece.Green, i);
                Assert.AreEqual(1, moves.Length, "Should have 1 option for roll " + i);
                Assert.AreEqual((Position)((byte)Position.GreenStart + i), moves[0].EndPosition, "Should move correct amount");
            }

            moves = start.GetMoves(Piece.Green, 6);
            Assert.AreEqual(2, moves.Length, "Should have 2 options for 6");
        }

        [Test]
        public void TestGreenShouldGetToGoal()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.Yellow6;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Green, 1);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Yellow6, moves[0].StartPosition);
            Assert.AreEqual(Position.GreenGoal1, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Green, 2);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Yellow6, moves[0].StartPosition);
            Assert.AreEqual(Position.GreenGoal2, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Green, 3);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Yellow6, moves[0].StartPosition);
            Assert.AreEqual(Position.GreenGoal3, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Green, 4);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Yellow6, moves[0].StartPosition);
            Assert.AreEqual(Position.GreenGoal4, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Green, 5);
            Assert.AreEqual(0, moves.Length);
        }

        [Test]
        public void TestRedShouldGetToGoal()
        {
            Position[] pos = getStartPositions();
            pos[4] = Position.Green5;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Red, 1);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Green5, moves[0].StartPosition);
            Assert.AreEqual(Position.Green6, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Red, 2);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Green5, moves[0].StartPosition);
            Assert.AreEqual(Position.RedGoal1, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Red, 3);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Green5, moves[0].StartPosition);
            Assert.AreEqual(Position.RedGoal2, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Red, 4);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Green5, moves[0].StartPosition);
            Assert.AreEqual(Position.RedGoal3, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Red, 5);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Green5, moves[0].StartPosition);
            Assert.AreEqual(Position.RedGoal4, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Red, 6);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.RedHome2, moves[0].StartPosition);
            Assert.AreEqual(Position.RedStart, moves[0].EndPosition);
        }

        [Test]
        public void TestBlueShouldGetToGoal()
        {
            Position[] pos = getStartPositions();
            pos[8] = Position.Red4;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Blue, 1);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Red4, moves[0].StartPosition);
            Assert.AreEqual(Position.Red5, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Blue, 2);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Red4, moves[0].StartPosition);
            Assert.AreEqual(Position.Red6, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Blue, 3);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Red4, moves[0].StartPosition);
            Assert.AreEqual(Position.BlueGoal1, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Blue, 4);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Red4, moves[0].StartPosition);
            Assert.AreEqual(Position.BlueGoal2, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Blue, 5);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Red4, moves[0].StartPosition);
            Assert.AreEqual(Position.BlueGoal3, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Blue, 6);
            Assert.AreEqual(2, moves.Length);
            Assert.AreEqual(Position.Red4, moves[0].StartPosition);
            Assert.AreEqual(Position.BlueGoal4, moves[0].EndPosition);
            Assert.AreEqual(Position.BlueHome2, moves[1].StartPosition);
            Assert.AreEqual(Position.BlueStart, moves[1].EndPosition);
        }

        [Test]
        public void TestYellowShouldGetToGoal()
        {
            Position[] pos = getStartPositions();
            pos[12] = Position.Blue3;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Yellow, 1);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Blue3, moves[0].StartPosition);
            Assert.AreEqual(Position.Blue4, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Yellow, 2);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Blue3, moves[0].StartPosition);
            Assert.AreEqual(Position.Blue5, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Yellow, 3);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Blue3, moves[0].StartPosition);
            Assert.AreEqual(Position.Blue6, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Yellow, 4);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Blue3, moves[0].StartPosition);
            Assert.AreEqual(Position.YellowGoal1, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Yellow, 5);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Blue3, moves[0].StartPosition);
            Assert.AreEqual(Position.YellowGoal2, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Yellow, 6);
            Assert.AreEqual(2, moves.Length);
            Assert.AreEqual(Position.Blue3, moves[0].StartPosition);
            Assert.AreEqual(Position.YellowGoal3, moves[0].EndPosition);
            Assert.AreEqual(Position.YellowHome2, moves[1].StartPosition);
            Assert.AreEqual(Position.YellowStart, moves[1].EndPosition);
        }

        [Test]
        public void TestGreenMovesInsideGoal()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.GreenGoal2;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Green, 1);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.GreenGoal2, moves[0].StartPosition);
            Assert.AreEqual(Position.GreenGoal3, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Green, 2);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.GreenGoal2, moves[0].StartPosition);
            Assert.AreEqual(Position.GreenGoal4, moves[0].EndPosition);

            moves = start.GetMoves(Piece.Green, 3);
            Assert.AreEqual(0, moves.Length);
        }

        [Test]
        public void TestYouCannotMoveOntoYourOwnPiece()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.Green6;
            pos[1] = Position.Green4;
            pos[2] = Position.Green2;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Green, 2);
            Assert.AreEqual(1, moves.Length, "Should only have 1 option, as others move onto your own piece");
            Assert.AreEqual(Position.Green6, moves[0].StartPosition);
            Assert.AreEqual(Position.Red1, moves[0].EndPosition);
        }

        [Test]
        public void TestEating()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.Red1;
            pos[4] = Position.Red4;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Green, 3);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Red1, moves[0].StartPosition);
            Assert.AreEqual(Position.Red4, moves[0].EndPosition);

            start.ApplyMove(moves[0]);
            Assert.AreEqual(Piece.None, start.board[(int)Position.Red1]);
            Assert.AreEqual(Piece.Green, start.board[(int)Position.Red4]);
            Assert.AreEqual(Piece.Red, start.board[(int)Position.RedHome1]);
            Assert.AreEqual(Piece.Red, start.board[(int)Position.RedHome2]);
            Assert.AreEqual(Piece.Red, start.board[(int)Position.RedHome4]);
            Assert.AreEqual(Piece.Red, start.board[(int)Position.RedHome3]);

            Assert.AreEqual(Position.RedHome1, start.pieces[4]);
        }

        [Test]
        public void TestEatingWithMultiplePiecesOnBoard()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.Red1;
            pos[4] = Position.GreenStart;
            pos[5] = Position.Yellow3;
            pos[6] = Position.Red2;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Green, 1);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Red1, moves[0].StartPosition);
            Assert.AreEqual(Position.Red2, moves[0].EndPosition);

            start.ApplyMove(moves[0]);
            Assert.AreEqual(Piece.None, start.board[(int)Position.Red1]);
            Assert.AreEqual(Piece.Green, start.board[(int)Position.Red2]);
            Assert.AreEqual(Piece.None, start.board[(int)Position.RedHome1]);
            Assert.AreEqual(Piece.None, start.board[(int)Position.RedHome2]);
            Assert.AreEqual(Piece.Red, start.board[(int)Position.RedHome4]);
            Assert.AreEqual(Piece.Red, start.board[(int)Position.RedHome3]);

            Assert.AreEqual(Position.RedHome3, start.pieces[6]);
        }

        [Test]
        public void TestAfterEatingHomePiecesShouldBeLastInPieceList()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.Yellow1;
            pos[4] = Position.GreenStart;
            pos[5] = Position.Yellow3;
            pos[6] = Position.Red2;
            Situation start = new Situation(pos);

            Move[] moves = start.GetMoves(Piece.Green, 2);
            start.ApplyMove(moves[0]);

            Assert.AreEqual(Position.GreenStart, start.pieces[4]);
            Assert.AreEqual(Position.Red2, start.pieces[5]);
            Assert.AreEqual(Position.RedHome3, start.pieces[6]);
            Assert.AreEqual(Position.RedHome4, start.pieces[7]);
        }

        [Test]
        public void TestNobodyWinsAtStart()
        {
            Situation start = Situation.GameStart();
            Assert.AreEqual(Piece.None, start.GetWinner());
        }

        [Test]
        public void TestGreenWins()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.GreenGoal2;
            pos[1] = Position.GreenGoal1;
            pos[2] = Position.GreenGoal4;
            pos[3] = Position.GreenGoal3;
            Situation start = new Situation(pos);

            Assert.AreEqual(Piece.Green, start.GetWinner());
        }

        [Test]
        public void TestYellowWins()
        {
            Position[] pos = getStartPositions();
            pos[12] = Position.YellowGoal1;
            pos[13] = Position.YellowGoal2;
            pos[14] = Position.YellowGoal4;
            pos[15] = Position.YellowGoal3;
            Situation start = new Situation(pos);

            Assert.AreEqual(Piece.Yellow, start.GetWinner());
        }

        [Test]
        public void TestAllHave3ThrowsAtStart()
        {
            Situation start = Situation.GameStart();
            Assert.AreEqual(3, start.GetNumberOfTries(Piece.Green));
            Assert.AreEqual(3, start.GetNumberOfTries(Piece.Red));
            Assert.AreEqual(3, start.GetNumberOfTries(Piece.Blue));
            Assert.AreEqual(3, start.GetNumberOfTries(Piece.Yellow));
        }

        [Test]
        public void TestOnly1ThrowNormally()
        {
            Position[] pos = getStartPositions();
            pos[12] = Position.YellowGoal3;
            Situation sit = new Situation(pos);

            Assert.AreEqual(1, sit.GetNumberOfTries(Piece.Yellow));
        }

        [Test]
        public void Test3ThrowsForFullyInGoal()
        {
            Position[] pos = getStartPositions();
            pos[4] = Position.RedGoal3;
            pos[5] = Position.RedGoal4;
            Situation sit = new Situation(pos);

            Assert.AreEqual(3, sit.GetNumberOfTries(Piece.Red));
        }

        [Test]
        public void TestCannotEatAPieceInStartPosition()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.Green6;
            pos[4] = Position.RedStart;

            Situation sit = new Situation(pos);

            Move[] moves = sit.GetMoves(Piece.Green, 1);
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Green6, moves[0].StartPosition);
            Assert.AreEqual(Position.GreenHome1, moves[0].EndPosition);
        }

        [Test]
        public void TestGreenCanDouble()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.GreenStart;

            Situation sit = new Situation(pos);

            Move[] moves = sit.GetMoves(Piece.Green, 6);
            Assert.AreEqual(2, moves.Length);
            Assert.AreEqual(Position.GreenStart, moves[0].StartPosition);
            Assert.AreEqual(Position.Green6, moves[0].EndPosition);
            Assert.AreEqual(Position.GreenHome2, moves[1].StartPosition);
            Assert.AreEqual(Position.GreenStart, moves[1].EndPosition);
            Assert.AreEqual(MoveType.DoubleUp, moves[1].Type);
        }

        [Test]
        public void TestDoublePieceMovesTogether()
        {
            Position[] pos = getStartPositions();
            pos[0] = Position.GreenStart;
            pos[1] = Position.GreenStart;

            Situation sit = new Situation(pos);
            Move[] moves = sit.GetMoves(Piece.Green, 3);
            Assert.AreEqual(1, moves.Length);
            sit.ApplyMove(moves[0]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenStart]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.Green3]);
            Assert.AreEqual(Position.Green3, sit.pieces[0]);
            Assert.AreEqual(Position.Green3, sit.pieces[1]);
        }

        [Test]
        public void TestRedCanTriple()
        {
            Position[] pos = getStartPositions();
            pos[4] = Position.RedStart;
            pos[5] = Position.RedStart;

            Situation sit = new Situation(pos);
            Move[] moves = sit.GetMoves(Piece.Red, 6);
            Assert.AreEqual(2, moves.Length);
            Assert.AreEqual(Position.RedStart, moves[0].StartPosition);
            Assert.AreEqual(Position.Red6, moves[0].EndPosition);
            Assert.AreEqual(Position.RedHome3, moves[1].StartPosition);
            Assert.AreEqual(Position.RedStart, moves[1].EndPosition);
            Assert.AreEqual(MoveType.DoubleUp, moves[1].Type);
        }

        [Test]
        public void TestRedCanQuadruple()
        {
            Position[] pos = getStartPositions();
            pos[4] = Position.RedStart;
            pos[5] = Position.RedStart;
            pos[6] = Position.RedStart;

            Situation sit = new Situation(pos);
            for (int i = 4; i < 7; i++)
            {
                Assert.AreEqual(Position.RedStart, sit.pieces[i]);
            }

            Move[] moves = sit.GetMoves(Piece.Red, 6);
            Assert.AreEqual(2, moves.Length);
            Assert.AreEqual(Position.RedStart, moves[0].StartPosition);
            Assert.AreEqual(Position.Red6, moves[0].EndPosition);
            Assert.AreEqual(Position.RedHome4, moves[1].StartPosition);
            Assert.AreEqual(Position.RedStart, moves[1].EndPosition);
            Assert.AreEqual(MoveType.DoubleUp, moves[1].Type);

            sit.ApplyMove(moves[1]);
            for (int i = 4; i < 8; i++)
            {
                Assert.AreEqual(Position.RedStart, sit.pieces[i]);
            }

            moves = sit.GetMoves(Piece.Red, 3);
            Assert.AreEqual(1, moves.Length);

            sit.ApplyMove(moves[0]);
            for (int i = 4; i < 8; i++)
            {
                Assert.AreEqual(Position.Red3, sit.pieces[i]);
            }
            Assert.AreEqual(Piece.None, sit.board[(int)Position.RedStart]);
            Assert.AreEqual(Piece.Red, sit.board[(int)Position.Red3]);
        }

        [Test]
        public void Test2DoublePieces()
        {
            var pos = getStartPositions();
            pos[0] = pos[1] = Position.Yellow2;
            pos[2] = pos[3] = Position.Red2;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 4);
            sit.ApplyMove(moves[1]);

            Assert.AreEqual(2, moves.Length);
            Assert.AreEqual(0, moves[0].Piece);
            Assert.AreEqual(Position.Yellow2, moves[0].StartPosition);
            Assert.AreEqual(Position.Yellow6, moves[0].EndPosition);
            Assert.AreEqual(2, moves[1].Piece);
            Assert.AreEqual(Position.Red2, moves[1].StartPosition);
            Assert.AreEqual(Position.Red6, moves[1].EndPosition);

            Assert.AreEqual(Position.Red6, sit.pieces[2]);
            Assert.AreEqual(Position.Red6, sit.pieces[3]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.Red2]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.Red6]);
        }

        [Test]
        public void TestDoubleArrivesToGoal()
        {
            var pos = getStartPositions();
            pos[0] = pos[1] = Position.Yellow6;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 1);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.GreenGoal1, moves[0].EndPosition);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal1]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenGoal2]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenGoal3]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenGoal4]);
            Assert.IsTrue(sit.Validate());
        }

        [Test]
        public void TestDoubleArrivesToGoal2()
        {
            var pos = getStartPositions();
            pos[0] = pos[1] = Position.Yellow6;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 2);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.GreenGoal2, moves[0].EndPosition);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal1]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal2]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenGoal3]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenGoal4]);
            Assert.IsTrue(sit.Validate());
        }

        [Test]
        public void TestDoubleArrivesToGoal3()
        {
            var pos = getStartPositions();
            pos[0] = pos[1] = Position.Yellow6;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 3);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.GreenGoal3, moves[0].EndPosition);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal1]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenGoal2]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal3]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenGoal4]);
            Assert.IsTrue(sit.Validate());
        }

        [Test]
        public void TestDoubleArrivesToGoalWithPieceAlreadyInGoal()
        {
            var pos = getStartPositions();
            pos[0] = Position.GreenGoal1;
            pos[1] = pos[2] = Position.Yellow6;


            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 4);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.GreenGoal4, moves[0].EndPosition);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal2]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenGoal3]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal4]);
        }

        [Test]
        public void TestTripleGetEaten()
        {
            var pos = getStartPositions();
            pos[0] = Position.GreenGoal1;
            pos[1] = pos[2] = pos[3] = Position.Yellow6;
            pos[4] = Position.Yellow3;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Red, 3);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual(Position.Yellow6, moves[0].EndPosition);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenHome1]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome2]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome3]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome4]);

            Assert.AreEqual(Position.GreenGoal1, sit.pieces[0]);
            Assert.AreEqual(Position.GreenHome2, sit.pieces[1]);
            Assert.AreEqual(Position.GreenHome3, sit.pieces[2]);
            Assert.AreEqual(Position.GreenHome4, sit.pieces[3]);
        }

        [Test]
        public void TestGreenGoesToGoal()
        {
            var pos = getStartPositions();
            pos[0] = Position.GreenGoal3;
            pos[1] = Position.Yellow6;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 1);

            Assert.AreEqual(2, moves.Length);
            sit.ApplyMove(moves[1]);

            Assert.AreEqual(Position.GreenGoal1, sit.pieces[1]);
            moves = sit.GetMoves(Piece.Green, 2);
            Assert.AreEqual(0, moves.Length);
        }

        [Test]
        public void TestCreateDoubleAndGetItEaten()
        {
            var sit = Situation.GameStart();
            var moves = sit.GetMoves(Piece.Green, 6);
            sit.ApplyMove(moves[0]);
            moves = sit.GetMoves(Piece.Green, 6);
            sit.ApplyMove(moves[1]);
            moves = sit.GetMoves(Piece.Green, 1);
            sit.ApplyMove(moves[0]);
            Assert.AreEqual(2, sit.GetPieceCount(Position.Green1));
            moves = sit.GetMoves(Piece.Yellow, 6);
            sit.ApplyMove(moves[0]);
            moves = sit.GetMoves(Piece.Yellow, 6);
            sit.ApplyMove(moves[0]);
            moves = sit.GetMoves(Piece.Yellow, 2);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome1]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome2]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome3]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome4]);

            Assert.AreEqual(0, sit.GetMoves(Piece.Green, 3).Length);
        }

        [Test]
        public void TestDoubleGetsEatenAndGetsOut()
        {
            var pos = getStartPositions();
            pos[0] = pos[1] = Position.RedStart;
            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Red, 6);
            sit.ApplyMove(moves[0]);
            moves = sit.GetMoves(Piece.Green, 6);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenStart]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.GreenHome1]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome2]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome3]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenHome4]);
            Assert.IsTrue(sit.Validate());
        }

        [Test]
        public void TestGeneratingMovesDoesNotBreakGroupings()
        {
            var pos = getStartPositions();
            pos[8] = pos[9] = Position.BlueGoal1;
            pos[10] = pos[11] = Position.BlueStart;

            var sit = new Situation(pos);

            Move[] moves = sit.GetMoves(Piece.Blue, 3);

            Assert.AreEqual(4, sit.groupingDict.Count);
            foreach (List<int> grouping in sit.groupingDict.Values)
            {
                Assert.AreEqual(2, grouping.Count);
            }
        }

        [Test]
        public void EveryoneHasZeroBeersAtStart()
        {
            var sit = new Situation();
            var sides = new Piece[] { Piece.Blue, Piece.Red, Piece.Green, Piece.Yellow };
            foreach(Piece side in sides) {
                Assert.AreEqual(0, sit.beers[(int)side]);
            }
        }

        [Test]
        public void RedShouldGetABeerWhenAPieceIsEaten()
        {
            var pos = getStartPositions();
            pos[0] = Position.Blue1;
            pos[5] = Position.Blue3;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 2);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(1, sit.beers[(int)Piece.Red]);
        }

        [Test]
        public void RedShouldGetTwoBeersWhenADoublePieceIsEaten()
        {
            var pos = getStartPositions();
            pos[0] = Position.Blue1;
            pos[5] = Position.Blue3;
            pos[6] = Position.Blue3;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 2);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(2, sit.beers[(int)Piece.Red]);
        }

        [Test]
        public void RedShouldGetTwoBeersWhenAPieceIsEatenByADouble()
        {
            var pos = getStartPositions();
            pos[0] = Position.Blue1;
            pos[1] = Position.Blue1;
            pos[5] = Position.Blue3;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 2);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(2, sit.beers[(int)Piece.Red]);
        }

        [Test]
        public void RedShouldGetFourBeersWhenADoublePieceIsEatenByADouble()
        {
            var pos = getStartPositions();
            pos[0] = Position.Blue1;
            pos[1] = Position.Blue1;
            pos[5] = Position.Blue3;
            pos[6] = Position.Blue3;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 2);
            sit.ApplyMove(moves[0]);

            Assert.AreEqual(4, sit.beers[(int)Piece.Red]);
        }

        [Test]
        public void DrinkingBeerLowersTheNumber()
        {
            var sit = new Situation();
            sit.beers[(int)Piece.Red] = 10;
            sit.DrinkBeers(Piece.Red, 3);
            Assert.AreEqual(7, sit.beers[(int)Piece.Red]);
        }

        [Test]
        public void CannotWinWhileTheresBeerLeft()
        {
            var pos = getStartPositions();
            pos[0] = Position.GreenGoal1;
            pos[1] = Position.GreenGoal2;
            pos[2] = Position.GreenGoal3;
            pos[3] = Position.GreenGoal4;

            var sit = new Situation(pos);
            sit.beers[(int)Piece.Green] = 1;

            Assert.AreEqual(Piece.None, sit.GetWinner());

            sit.DrinkBeers(Piece.Green, 1);

            Assert.AreEqual(Piece.Green, sit.GetWinner());
        }

        [Test]
        public void DoubleUnfoldsWhenPiecesBeforeItAreFilled()
        {
            var pos = getStartPositions();
            pos[0] = Position.GreenGoal4;
            pos[1] = pos[2] = Position.GreenGoal2;
            pos[3] = Position.GreenGoal1;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 2);

            sit.ApplyMove(moves[0]);

            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal1]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal2]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal3]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal4]);

            Assert.AreEqual(Piece.Green, sit.GetWinner());
        }

        [Test]
        public void TripleFinishesOverOwnPiece()
        {
            var pos = getStartPositions();
            pos[0] = Position.GreenGoal2;
            pos[1] = pos[2] = pos[3] = Position.GreenGoal1;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Green, 3);

            sit.ApplyMove(moves[0]);

            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal1]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal2]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal3]);
            Assert.AreEqual(Piece.Green, sit.board[(int)Position.GreenGoal4]);

            Assert.AreEqual(Piece.Green, sit.GetWinner());
        }

        [Test]
        public void DoubleUnfoldsToStartPosition()
        {
            var pos = getStartPositions();
            pos[4] = pos[5] = Position.RedGoal1;

            var sit = new Situation(pos);
            var moves = sit.GetMoves(Piece.Red, 3);

            Assert.AreEqual(1, moves.Length);

            sit.ApplyMove(moves[0]);

            Assert.AreEqual(Piece.Red, sit.board[(int)Position.RedGoal1]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.RedGoal2]);
            Assert.AreEqual(Piece.None, sit.board[(int)Position.RedGoal3]);
            Assert.AreEqual(Piece.Red, sit.board[(int)Position.RedGoal4]);
        }

        private static Position[] getStartPositions()
        {
            return new Position[] { Position.GreenHome1, Position.GreenHome2, Position.GreenHome3, Position.GreenHome4,
                                    Position.RedHome1, Position.RedHome2,Position.RedHome3,Position.RedHome4,
                                    Position.BlueHome1,Position.BlueHome2,Position.BlueHome3,Position.BlueHome4,
                                    Position.YellowHome1,Position.YellowHome2,Position.YellowHome3,Position.YellowHome4};
        }
    }
}

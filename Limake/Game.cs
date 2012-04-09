using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public class Game
    {
        private Situation situation;
        private Random rand;
        private IGameDisplay display;
        private IPlayer[] players;

        public List<HistoryEntry> history;

        public Game(IGameDisplay display, IPlayer[] players)
        {
            rand = new Random();
            if (players == null || players.Length != 4)
            {
                throw new Exception("Need 4 players to play");
            }
            this.display = display;
            this.players = players;
            this.history = new List<HistoryEntry>();

            situation = Situation.GameStart();
        }

        public Piece Run()
        {
            int turn = 0;
            while (situation.GetWinner() == Piece.None)
            {
                Piece currentSide = (Piece)((turn % 4) + 1);
                display.DisplayTurn(currentSide, turn);

                for (int side = 1; side < 5; side++)
                {
                    if (situation.beers[side] > 0)
                    {
                        Piece sidePiece = (Piece)(side);
                        int beers = players[side-1].HowManyBeersAreDrunk(sidePiece);
                        situation.DrinkBeers(sidePiece, beers);
                    }
                }
                bool didMove = false;
                int triesLeft = situation.GetNumberOfTries(currentSide);
                do
                {
                    int roll = 0;
                    do
                    {
                        display.DisplaySituation(situation);
                        if (triesLeft == 0)
                        {
                            break;
                        }

                        players[turn % 4].WaitForRoll();
                        roll = PopOMatic();
                        display.DisplayRoll(roll);
                        Move[] moves = situation.GetMoves(currentSide, roll);
                        if (moves.Length == 0)
                        {
                            players[turn % 4].NoMovesAvailable(situation);
                            display.DisplayNoMove();
                        }
                        else
                        {
                            int moveSelected = players[turn % 4].SelectMove(situation, moves, currentSide, roll);
                            Move move = moves[moveSelected];
                            this.history.Add(new HistoryEntry(new Situation(situation), move));
                            situation.ApplyMove(move);
                            display.DispĺayMove(move);
                            if (!situation.Validate())
                            {
                                throw new LimakeException(situation.Errors);
                            }
                            didMove = true;
                        }
                        // A 6 gets a new turn
                    } while (roll == 6);
                } while (!didMove && --triesLeft > 0);


                turn++;
            }

            display.DisplaySituation(situation);
            return situation.GetWinner();
        }

        private int PopOMatic()
        {
            return rand.Next(1, 7);
        }
    }

    public class HistoryEntry
    {
        public Situation situation;
        public Move move;
        public HistoryEntry(Situation sit, Move move)
        {
            this.situation = sit;
            this.move = move;
        }
    }
}

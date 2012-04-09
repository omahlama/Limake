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
using Limake;
using System.Threading;

namespace LimakeSilverLightUI
{
    public delegate void SelectMoveHandler(Situation situation, Move[] moves, Piece side, int roll);
    public delegate void NoMovesAvailableHandler(Situation situation);
    public delegate void DisplayTurnHandler(Piece side, int turn);
    public delegate void DisplayRollHandler(int roll);
    public delegate void DisplaySituationHandler(Situation situation);
    public delegate void DisplayMoveHandler(Move move);
    public delegate void DisplayNoMoveHandler();
    public delegate void BeersAcceptedHandler(Piece side, int count);
    public delegate void GameOverHandler(Piece winner);
    public delegate void ErrorHandler(String error);
    public delegate void WaitForRollHandler();

    public class ThreadedLimakeGame : IPlayer, IGameDisplay
    {
        public SelectMoveHandler SelectMove;
        public NoMovesAvailableHandler NoMovesAvailable;
        public DisplayTurnHandler DisplayTurn;
        public DisplayRollHandler DisplayRoll;
        public DisplaySituationHandler DisplaySituation;
        public DisplayMoveHandler DisplayMove;
        public DisplayNoMoveHandler DisplayNoMove;
        public BeersAcceptedHandler BeersAccepted;
        public GameOverHandler GameOver;
        public ErrorHandler Error;
        public WaitForRollHandler WaitForRoll;

        public const int delayAmount = 500;

        private Game game;
        Thread thread;
        private volatile int selectedMove;
        private int[] beersDrunk;

        public ThreadedLimakeGame()
        {
            beersDrunk = new int[5];

            IPlayer[] players = new IPlayer[] { new BasicPlayer(), new BasicPlayer(), this, new BasicPlayer() };
            game = new Game(this, players);
        }

        public void Run()
        {
            thread = new Thread(DoRun);
            thread.Start();
        }

        public void DrankBeer(Piece side, int amount)
        {
            beersDrunk[(int)side] += amount;
        }

        private void DoRun()
        {
            try
            {
                Piece winner = game.Run();
                if (GameOver != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        GameOver(winner);
                    });
                }
            }
            catch (LimakeException e)
            {
                if (Error != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        Error(e.Errors);
                    });
                }
            }
        }

        public void SetSelectedMove(int moveIndex)
        {
            this.selectedMove = moveIndex;
            moveARE.Set();
        }

        private AutoResetEvent moveARE = new AutoResetEvent(false);
        int IPlayer.SelectMove(Situation situation, Move[] moves, Piece side, int roll)
        {
            selectedMove = -1;
            moveARE.Reset();
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                SelectMove(situation, moves, side, roll);
            });
            moveARE.WaitOne();
            return selectedMove;
        }

        void IPlayer.NoMovesAvailable(Situation situation)
        {
            if (NoMovesAvailable != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    NoMovesAvailable(situation);
                });
                Delay(false);
            }
        }

        int IPlayer.HowManyBeersAreDrunk(Piece side)
        {
            int beers = beersDrunk[(int)side];
            beersDrunk[(int)side] = 0;
            if (BeersAccepted != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    BeersAccepted(side, beers)
                );
            }
            return beers;
        }

        private AutoResetEvent rollARE = new AutoResetEvent(false);
        void IPlayer.WaitForRoll()
        {
            rollARE.Reset();

            if (WaitForRoll != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    WaitForRoll()
                );
            }

            rollARE.WaitOne();
        }

        public void Rolled()
        {
            rollARE.Set();
        }


        private void Delay(bool isLong)
        {
            Thread.Sleep(isLong ? delayAmount : 50);
        }

        void IGameDisplay.DisplayTurn(Piece side, int turn)
        {
            if (DisplayTurn != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    DisplayTurn(side, turn);
                });
                Delay(false);
            }
        }

        void IGameDisplay.DisplayRoll(int Roll)
        {
            if (DisplayRoll != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    DisplayRoll(Roll);
                });
                Delay(false);
            }
        }

        void IGameDisplay.DisplaySituation(Situation situation)
        {
            if (DisplaySituation != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    DisplaySituation(situation);
                });
                Delay(true);
            }
        }

        void IGameDisplay.DispĺayMove(Move move)
        {
            if (DisplayMove != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    DisplayMove(move);
                });
                Delay(false);
            }
        }

        void IGameDisplay.DisplayNoMove()
        {
            if (DisplayNoMove != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    DisplayNoMove();
                });
                Delay(false);
            }
        }
    }
}

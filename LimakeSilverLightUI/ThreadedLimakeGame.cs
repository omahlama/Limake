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
    public delegate void GameOverHandler(Piece winner);
    public delegate void ErrorHandler(String error);

    public class ThreadedLimakeGame : IPlayer, IGameDisplay
    {
        public SelectMoveHandler SelectMove;
        public NoMovesAvailableHandler NoMovesAvailable;
        public DisplayTurnHandler DisplayTurn;
        public DisplayRollHandler DisplayRoll;
        public DisplaySituationHandler DisplaySituation;
        public DisplayMoveHandler DisplayMove;
        public DisplayNoMoveHandler DisplayNoMove;
        public GameOverHandler GameOver;
        public ErrorHandler Error;

        public const int delayAmount = 500;

        private Game game;
        Thread thread;
        private volatile int selectedMove;


        public ThreadedLimakeGame()
        {
            IPlayer[] players = new IPlayer[] { new BasicPlayer(), new BasicPlayer(), this, new BasicPlayer() };
            game = new Game(this, players);
        }

        public void Run()
        {
            thread = new Thread(DoRun);
            thread.Start();
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
        }

        int IPlayer.SelectMove(Situation situation, Move[] moves, Piece side, int roll)
        {
            selectedMove = -1;
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                SelectMove(situation, moves, side, roll);
            });
            while (selectedMove == -1)
            {
                Thread.Sleep(100);
            }
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
            return 0;
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

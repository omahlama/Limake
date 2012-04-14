using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Limake;
using System.Threading;
using System.Windows.Navigation;

namespace LimakeSilverLightUI
{
    public partial class MainPage : Page
    {
        private ThreadedLimakeGame game;
        private Move[] availableMoves;
        private Situation currentSituation;

        private PieceControl[] pieces;
        private PositionControl[] positionControls;

        private PlayerType green, red, blue, yellow;

        public MainPage()
        {
            InitializeComponent();

            InitializePositionControls();

            this.LayoutUpdated += new EventHandler(MainPage_LayoutUpdated);
        }

        private PlayerType ParseType(String key)
        {
            return (PlayerType)Enum.Parse(typeof(PlayerType), NavigationContext.QueryString[key], true);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            green = ParseType("Green");
            red = ParseType("Red");
            blue = ParseType("Blue");
            yellow = ParseType("Yellow");
            game = new ThreadedLimakeGame(green, red, blue, yellow);
            game.DisplaySituation += DisplaySituationHandler;
            game.DisplayRoll += DisplayRollHandler;
            game.DisplayTurn += DisplayTurnHandler;
            game.SelectMove += SelectMoveHandler;
            game.BeersAccepted += BeersAcceptedHandler;
            game.GameOver += GameOverHandler;
            game.WaitForRoll += WaitForRollHandler;
            game.Run();
        }

        private bool firstLayout = true;

        void MainPage_LayoutUpdated(object sender, EventArgs e)
        {
            if (firstLayout)
            {
                firstLayout = false;
                InitializePieces();

                PlayerType[] players = new PlayerType[] { green, red, blue, yellow };
                BeerDisplay[] beers = new BeerDisplay[] { GreenBeer, RedBeer, BlueBeer, YellowBeer };

                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] == PlayerType.Human)
                    {
                        beers[i].Interactive = true;
                        beers[i].DrankBeer += DrankBeer;
                    }
                }

                die.Rolled += DieRolled;
            }
        }

        private void InitializePieces()
        {
            pieces = new PieceControl[16];
            for (int i = 0; i < 16; i++)
            {
                PieceInfo pi = new PieceInfo();
                pi.Piece = (Piece)((i / 4)+1);
                switch (pi.Piece)
                {
                    case Piece.Green: pi.PieceColor = Colors.Green; break;
                    case Piece.Red: pi.PieceColor = Colors.Red; break;
                    case Piece.Blue: pi.PieceColor = Colors.Blue; break;
                    case Piece.Yellow: pi.PieceColor = Colors.Yellow; break;
                }
                pi.Number = i;
                pieces[i] = new PieceControl();
                pieces[i].DataContext = pi;
                pieces[i].PieceSelected += new PieceSelectedHandler(PieceSelected);

                Position position = (Position)(i + (int)Position.GreenHome1);

                Point offset = GetAbsolutePosition(position);

                Canvas.SetTop(pieces[i], offset.Y);
                Canvas.SetLeft(pieces[i], offset.X);
                mainCanvas.Children.Add(pieces[i]);
            }
        }

        private Point GetAbsolutePosition(Position position)
        {
            GeneralTransform gt = positionControls[(int)position].TransformToVisual(mainCanvas);
            Point offset = gt.Transform(new Point(0, 0));
            offset.Y -= 30;
            return offset;
        }

        private void InitializePositionControls()
        {
            positionControls = new PositionControl[(int)Position.None];
            for (int i = 0; i < (int)Position.None; i++)
            {
                PositionInfo pi = new PositionInfo();
                pi.Name = ((Position)i).ToString();
                PositionControl pc = new PositionControl();
                pc.DataContext = pi;
                pc.Name = pi.Name;

                positionControls[i] = pc;

                if (i < (int)Position.GreenGoal1)
                {
                    switch((Position)i) {
                        case Position.GreenStart: pi.Color = Colors.Green; break;
                        case Position.RedStart: pi.Color = Colors.Red; break;
                        case Position.BlueStart: pi.Color = Colors.Blue; break;
                        case Position.YellowStart: pi.Color = Colors.Yellow; break;
                        default: pi.Color = Colors.White; break;
                    }
                    boardPanel.Children.Add(pc);
                }
                else if (i < (int)Position.RedGoal1)
                {
                    pi.Color = Colors.Green;
                    greenGoalPanel.Children.Add(pc);
                }
                else if (i < (int)Position.BlueGoal1)
                {
                    pi.Color = Colors.Red;
                    redGoalPanel.Children.Add(pc);
                }
                else if (i < (int)Position.YellowGoal1)
                {
                    pi.Color = Colors.Blue;
                    blueGoalPanel.Children.Add(pc);
                }
                else if (i < (int)Position.GreenHome1)
                {
                    pi.Color = Colors.Yellow;
                    yellowGoalPanel.Children.Add(pc);
                }
                else if (i < (int)Position.RedHome1)
                {
                    pi.Color = Colors.Green;
                    greenHomePanel.Children.Add(pc);
                }
                else if (i < (int)Position.BlueHome1)
                {
                    pi.Color = Colors.Red;
                    redHomePanel.Children.Add(pc);
                }
                else if (i < (int)Position.YellowHome1)
                {
                    pi.Color = Colors.Blue;
                    blueHomePanel.Children.Add(pc);
                }
                else
                {
                    pi.Color = Colors.Yellow;
                    yellowHomePanel.Children.Add(pc);
                }
            }
        }

        private void DisplaySituationHandler(Situation situation)
        {
            CreateStoryBoard(situation.HasMultistepAnimation());
            for (int i = 0; i < situation.animations.Length; i++)
            {
                if (situation.animations[i] != null)
                {
                    foreach (Situation.Animation animation in situation.animations[i])
                    {
                        AnimatePieceToPosition(animation.Piece, animation.Start, animation.End, i);
                    }
                }
            }
            StartAnimation();
            situation.ClearAnimations();

            // When animation is done, move all pieces to correct positions to account for non-animated moves
            board.Completed += (sender, args) =>
            {
                for (int i = 0; i < situation.pieces.Length; i++)
                {
                    Point p = GetAbsolutePosition(situation.pieces[i]);
                    Canvas.SetLeft(pieces[i], p.X);
                    Canvas.SetTop(pieces[i], p.Y);
                }
            };

            GreenBeer.BeerCount = situation.beers[(int)Piece.Green];
            RedBeer.BeerCount = situation.beers[(int)Piece.Red];
            BlueBeer.BeerCount = situation.beers[(int)Piece.Blue];
            YellowBeer.BeerCount = situation.beers[(int)Piece.Yellow];

            GreenTurnsLabel.Content = SinglePlayerData.Instance.GetTurnsRemainingExpectedValue(situation, Piece.Green).ToString("f2");
            RedTurnsLabel.Content = SinglePlayerData.Instance.GetTurnsRemainingExpectedValue(situation, Piece.Red).ToString("f2");
            BlueTurnsLabel.Content = SinglePlayerData.Instance.GetTurnsRemainingExpectedValue(situation, Piece.Blue).ToString("f2");
            YellowTurnsLabel.Content = SinglePlayerData.Instance.GetTurnsRemainingExpectedValue(situation, Piece.Yellow).ToString("f2");
        }

        private void DisplayRollHandler(int roll)
        {
            die.SetNumber(roll);
        }

        private void DisplayTurnHandler(Piece side, int turn)
        {
            TurnDisplay.SetTurn(turn);
        }

        // Event handlers for threadedlimakegame
        private void SelectMoveHandler(Situation situation, Move[] moves, Piece side, int roll)
        {
            this.availableMoves = moves;
            this.currentSituation = situation;

            for (int i = 0; i < moves.Length; i++)
            {
                Move move = moves[i];
                pieces[move.Piece].Movable = true;
                if (situation.groupingDict.ContainsKey(move.Piece))
                {
                    foreach (int otherPiece in situation.groupingDict[move.Piece])
                    {
                        if (otherPiece != move.Piece)
                        {
                            pieces[otherPiece].Movable = true;
                        }
                    }
                }
            }
        }

        private void PieceSelected(PieceControl pieceControl)
        {
            PieceInfo pi = pieceControl.DataContext as PieceInfo;

            for (int i = 0; i < this.availableMoves.Length; i++)
            {
                if (this.availableMoves[i].Piece == pi.Number || (currentSituation.groupingDict.ContainsKey(pi.Number) && currentSituation.groupingDict[pi.Number].Contains(this.availableMoves[i].Piece)))
                {
                    ClearSelectionOptions();
                    game.SetSelectedMove(i);
                    break;
                }
            }
        }

        private void ClearSelectionOptions()
        {
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].Movable = false;
            }
        }

        private Storyboard board;
        private Duration duration;
        private Dictionary<int, DoubleAnimationUsingKeyFrames> leftAnimationDict, topAnimationDict;
        private void CreateStoryBoard(bool multistep)
        {
            board = new Storyboard();
            duration = new Duration(TimeSpan.FromMilliseconds(ThreadedLimakeGame.delayAmount));
            board.Duration = new Duration(TimeSpan.FromMilliseconds(ThreadedLimakeGame.delayAmount*(multistep?2:1)));
            board.Completed += (sender, args) => game.DisplaySituationComplete();

            leftAnimationDict = new Dictionary<int, DoubleAnimationUsingKeyFrames>();
            topAnimationDict = new Dictionary<int, DoubleAnimationUsingKeyFrames>();
        }

        private int animationCounter = 0;
        private void StartAnimation()
        {
            mainCanvas.Resources.Add("Animation_" + (animationCounter++), board);
            board.Begin();
            leftAnimationDict = new Dictionary<int, DoubleAnimationUsingKeyFrames>();
            topAnimationDict = new Dictionary<int, DoubleAnimationUsingKeyFrames>();
        }

        private void AnimatePieceToPosition(int piece, Position start, Position end, int phase)
        {
            TimeSpan beginTime = TimeSpan.FromMilliseconds(ThreadedLimakeGame.delayAmount * phase),
                    endTime = TimeSpan.FromMilliseconds(ThreadedLimakeGame.delayAmount * (phase+1));

            PieceControl pieceControl = pieces[piece];

            Point startpoint = GetAbsolutePosition(start);
            Point endpoint = GetAbsolutePosition(end);
            
            DoubleAnimationUsingKeyFrames leftAnimation, topAnimation;
            if (leftAnimationDict.ContainsKey(piece))
            {
                leftAnimation = leftAnimationDict[piece];
            }
            else
            {
                leftAnimation = new DoubleAnimationUsingKeyFrames();
                leftAnimationDict[piece] = leftAnimation;
                board.Children.Add(leftAnimation);

                Storyboard.SetTarget(leftAnimation, pieceControl);
                Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("(Canvas.Left)"));
            }

            if (topAnimationDict.ContainsKey(piece))
            {
                topAnimation = topAnimationDict[piece];
            }
            else
            {
                topAnimation = new DoubleAnimationUsingKeyFrames();
                topAnimationDict[piece] = topAnimation;
                board.Children.Add(topAnimation);

                Storyboard.SetTarget(topAnimation, pieceControl);
                Storyboard.SetTargetProperty(topAnimation, new PropertyPath("(Canvas.Top)"));
            }

            LinearDoubleKeyFrame leftStart = new LinearDoubleKeyFrame()
            {
                Value = startpoint.X,
                KeyTime = KeyTime.FromTimeSpan(beginTime)
            };
            LinearDoubleKeyFrame leftEnd = new LinearDoubleKeyFrame()
            {
                Value = endpoint.X,
                KeyTime = KeyTime.FromTimeSpan(endTime)
            };
            leftAnimation.KeyFrames.Add(leftStart);
            leftAnimation.KeyFrames.Add(leftEnd);

            LinearDoubleKeyFrame topStart = new LinearDoubleKeyFrame()
            {
                Value = startpoint.Y,
                KeyTime = KeyTime.FromTimeSpan(beginTime)
            };
            LinearDoubleKeyFrame topEnd = new LinearDoubleKeyFrame()
            {
                Value = endpoint.Y,
                KeyTime = KeyTime.FromTimeSpan(endTime)
            };
            topAnimation.KeyFrames.Add(topStart);
            topAnimation.KeyFrames.Add(topEnd);
        }

        public void DrankBeer(object sender, EventArgs args)
        {
            BeerDisplay[] beers = new BeerDisplay[] { GreenBeer, RedBeer, BlueBeer, YellowBeer };
            int i;
           
            for (i = 0; i < beers.Length; i++)
            {
                if (sender == beers[i])
                    break;
            }
            
            game.DrankBeer((Piece)(i+1), 1);
        }

        private void BeersAcceptedHandler(Piece side, int amount)
        {
            BeerDisplay[] beers = new BeerDisplay[] { GreenBeer, RedBeer, BlueBeer, YellowBeer };
            beers[(int)side - 1].DrankCount -= amount;
        }

        private void GameOverHandler(Piece winner)
        {
            if (winner == Piece.Blue)
            {
                WinnerLabel.Content = "You won!";
            }
            else
            {
                WinnerLabel.Content = "You lost, winner is " + winner;
            }
            WinnerOverlay.Visibility = System.Windows.Visibility.Visible;
        }

        private void WaitForRollHandler()
        {
            die.Active = true;
        }

        private void DieRolled(Object sender, EventArgs args)
        {
            game.Rolled();
        }
    }

    public class PositionInfo
    {
        public String Name { get; set; }
        public Color Color { get; set; }
    }

    public class PieceInfo
    {
        public Color PieceColor { get; set; }
        public Piece Piece { get; set; }
        public int Number { get; set; }
    }
}

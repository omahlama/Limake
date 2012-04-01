﻿using System;
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

namespace LimakeSilverLightUI
{
    public partial class MainPage : UserControl
    {
        private ThreadedLimakeGame game;
        private Move[] availableMoves;
        private Situation currentSituation;

        private PieceControl[] pieces;
        private PositionControl[] positionControls;

        public MainPage()
        {
            InitializeComponent();

            InitializePositionControls();

            this.LayoutUpdated += new EventHandler(MainPage_LayoutUpdated);

            game = new ThreadedLimakeGame();
            game.DisplaySituation += DisplaySituationHandler;
            game.DisplayRoll += DisplayRollHandler;
            game.SelectMove += SelectMoveHandler;
            game.Run();

            Application.Current.Host.Content.Resized += new EventHandler(Content_Resized);
        }

        // Make this game scale to fit the space it is given
        private void Content_Resized(object sender, EventArgs e)
        {
            double scale = Math.Min(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight) / 800;
            ScaleTransform st = new ScaleTransform();
            st.ScaleX = scale;
            st.ScaleY = scale;
            this.RenderTransform = st;
        }

        private bool firstLayout = true;

        void MainPage_LayoutUpdated(object sender, EventArgs e)
        {
            if (firstLayout)
            {
                firstLayout = false;
                InitializePieces();

                AnimatePieceToPosition(4, Position.Red5);
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
                    boardPanel.Children.Add(pc);
                }
                else if (i < (int)Position.RedGoal1)
                {
                    greenGoalPanel.Children.Add(pc);
                }
                else if (i < (int)Position.BlueGoal1)
                {
                    redGoalPanel.Children.Add(pc);
                }
                else if (i < (int)Position.YellowGoal1)
                {
                    blueGoalPanel.Children.Add(pc);
                }
                else if (i < (int)Position.GreenHome1)
                {
                    yellowGoalPanel.Children.Add(pc);
                }
                else if (i < (int)Position.RedHome1)
                {
                    greenHomePanel.Children.Add(pc);
                }
                else if (i < (int)Position.BlueHome1)
                {
                    redHomePanel.Children.Add(pc);
                }
                else if (i < (int)Position.YellowHome1)
                {
                    blueHomePanel.Children.Add(pc);
                }
                else
                {
                    yellowHomePanel.Children.Add(pc);
                }
            }
        }

        private void DisplaySituationHandler(Situation situation)
        {
            for (int i = 0; i < situation.pieces.Length; i++)
            {
                AnimatePieceToPosition(i, situation.pieces[i]);
            }
        }

        private void DisplayRollHandler(int roll)
        {
            die.SetNumber(roll);
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

        private void AnimatePieceToPosition(int piece, Position position)
        {
            PieceControl pieceControl = pieces[piece];
            PositionControl positionControl = positionControls[(int)position];

            Point point = GetAbsolutePosition(position);

            Storyboard board = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromMilliseconds(ThreadedLimakeGame.delayAmount));
            board.Duration = duration;

            ExponentialEase ease = new ExponentialEase();
            ease.Exponent = 2;
            ease.EasingMode = EasingMode.EaseInOut;

            DoubleAnimation leftAnimation = new DoubleAnimation();
            DoubleAnimation topAnimation = new DoubleAnimation();

            leftAnimation.Duration = duration;
            topAnimation.Duration = duration;

            leftAnimation.EasingFunction = ease;
            topAnimation.EasingFunction = ease;

            board.Children.Add(leftAnimation);
            board.Children.Add(topAnimation);

            Storyboard.SetTarget(leftAnimation, pieceControl);
            Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("(Canvas.Left)"));

            Storyboard.SetTarget(topAnimation, pieceControl);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath("(Canvas.Top)"));

            leftAnimation.To = point.X;
            topAnimation.To = point.Y;

            mainCanvas.Resources.Add("Animation_" + (animationCounter++), board);
            board.Begin();
        }

        private int animationCounter = 0;
    }

    public class PositionInfo
    {
        public String Name { get; set; }
    }

    public class PieceInfo
    {
        public Color PieceColor { get; set; }
        public Piece Piece { get; set; }
        public int Number { get; set; }
    }
}

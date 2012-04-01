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

namespace LimakeSilverLightUI
{
    public delegate void MoveSelectedHandler(Object sender);

    public partial class PositionUI : UserControl
    {
        public static readonly DependencyProperty FillBrushProperty = DependencyProperty.Register(
            "FillBrush", typeof(Brush), typeof(PositionUI), null);

        public static readonly DependencyProperty NoticeBrushProperty = DependencyProperty.Register(
            "NoticeBrush", typeof(Brush), typeof(PositionUI), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public MoveSelectedHandler MoveSelected;

        public PositionUI()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(PositionUI_MouseLeftButtonDown);
        }

        void PositionUI_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.StartPiece && MoveSelected != null)
            {
                MoveSelected(this);
            }
        }

        private bool startPiece;
        public bool StartPiece
        {
            get { return startPiece; }
            set { startPiece = value; UpdateBorderColor(); }
        }

        private bool endPiece;
        public bool EndPiece
        {
            get { return endPiece; }
            set { endPiece = value; UpdateBorderColor(); }
        }

        private Piece piece;
        public Piece Piece
        {
            set { this.piece = value; this.FillBrush = BrushForPiece(value);}
            get { return this.piece; }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set 
            {
                count = value;
                if (value > 1)
                {
                    CountBox.Text = "" + count;
                }
                else
                {
                    CountBox.Text = "";
                }
            }
        }

        public Brush FillBrush
        {
            set { this.SetValue(FillBrushProperty, value); }
            get { return (Brush)this.GetValue(FillBrushProperty); }
        }

        public Brush NoticeBrush
        {
            set { this.SetValue(NoticeBrushProperty, value); }
            get { return (Brush)this.GetValue(NoticeBrushProperty); }            
        }

        private Brush BrushForPiece(Piece piece)
        {
            Color color = Colors.Transparent;
            switch (piece)
            {
                case Piece.Red:
                    color = Colors.Red;
                    break;
                case Piece.Green:
                    color = Colors.Green;
                    break;
                case Piece.Yellow:
                    color = Colors.Yellow;
                    break;
                case Piece.Blue:
                    color = Colors.Blue;
                    break;
            }
            return new SolidColorBrush(color);
        }

        private void UpdateBorderColor()
        {
            Color color = Colors.Black;
            if (this.StartPiece)
            {
                color = Colors.White;
            } else if (this.EndPiece)
            {
                color = Colors.Brown;
            }

            var g = new GradientStopCollection();
            var s = new GradientStop();
            s.Color = Colors.Black;
            s.Offset = 0;
            g.Add(s);
            s = new GradientStop();
            s.Color = color;
            s.Offset = 1;
            g.Add(s);

            this.NoticeBrush = new LinearGradientBrush(g, 45);
        }
    }
}

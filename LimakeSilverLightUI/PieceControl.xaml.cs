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
    public delegate void PieceSelectedHandler(PieceControl sender);

    public partial class PieceControl : UserControl
    {
        public PieceSelectedHandler PieceSelected;

        public PieceControl()
        {
            InitializeComponent();
        }

        private bool movable = false;
        public bool Movable
        {
            get { return movable; }
            set
            {
                this.movable = value;
                if (value)
                {
                    movableEffect.Opacity = 1;
                }
                else
                {
                    movableEffect.Opacity = 0;
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.Movable && this.PieceSelected != null)
            {
                this.PieceSelected(this);
            }
            e.Handled = false;
            base.OnMouseLeftButtonDown(e);
        }
    }
}

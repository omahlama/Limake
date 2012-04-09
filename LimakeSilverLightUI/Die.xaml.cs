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
    public partial class Die : UserControl
    {
        public EventHandler Rolled;
        private Random rand = new Random();

        public Die()
        {
            InitializeComponent();
            UpdateEllipse();
        }

        private double randRange(double limit)
        {
            return (rand.NextDouble() - 0.5) * 2 * limit;  
        }

        public void SetNumber(int number)
        {
            StartAngle.Value = EndAngle.Value;
            Angle1.Value = StartAngle.Value + randRange(90);
            Angle2.Value = Angle1.Value + randRange(90);
            EndAngle.Value = Angle2.Value + randRange(90);

            StartX.Value = EndX.Value;
            X1.Value = randRange(20);
            X2.Value = randRange(20);
            EndX.Value = randRange(20);

            StartY.Value = EndY.Value;
            Y1.Value = randRange(20);
            Y2.Value = randRange(20);
            EndY.Value = randRange(20);

            N1.Value = -64 * rand.Next(6);
            EndN.Value = -64 * (number - 1);

            RollAnimation.Begin();
        }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { active = value; UpdateEllipse();  }
        }

        private void UpdateEllipse()
        {
            if (this.active)
                EllipseBorderStop.Color = Color.FromArgb(255, 32, 32, 32);
            else
                EllipseBorderStop.Color = Color.FromArgb(255, 196, 196, 196);
        }

        private void Circle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Active)
            {
                this.Active = false;
                if (Rolled != null)
                {
                    Rolled(this, new EventArgs());
                }
            }
        }
    }
}

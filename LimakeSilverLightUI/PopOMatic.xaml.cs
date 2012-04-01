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
    public partial class PopOMatic : UserControl
    {
        public static readonly DependencyProperty RollProperty = DependencyProperty.Register(
    "FillBrush", typeof(string), typeof(PopOMatic), null);

        public string Roll
        {
            set
            {
                this.SetValue(RollProperty, value);
            }
            get
            {
                return (string)this.GetValue(RollProperty);
            }
        }

        public PopOMatic()
        {
            InitializeComponent();
        }
    }
}

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
    public partial class Root : UserControl
    {
        public Root()
        {
            InitializeComponent();

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

    }
}

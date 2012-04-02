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
using System.Windows.Media.Imaging;

namespace LimakeSilverLightUI
{
    public partial class BeerDisplay : UserControl
    {
        public BeerDisplay()
        {
            InitializeComponent();
        }

        private int beerCount = 0;
        public int BeerCount
        {
            get { return beerCount; }
            set
            {
                beerCount = value;
                this.UpdateBeers();
            }
        }

        private void UpdateBeers()
        {
            Grid g = this.LayoutRoot;
            int cols = (int)Math.Ceiling(Math.Sqrt(this.beerCount));
            int rows = 0;
            while(cols * rows < beerCount) {
                rows++;
            }

            g.Children.Clear();
            g.ColumnDefinitions.Clear();
            g.RowDefinitions.Clear();

            for (int i = 0; i < cols; i++)
                g.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < rows; i++)
                g.RowDefinitions.Add(new RowDefinition());

            ImageSource source = new BitmapImage( new Uri("/LimakeSilverLightUI;component/beer.png", UriKind.Relative) );
            for (int i = 0; i < beerCount; i++)
            {
                Image image = new Image();
                image.Source = source;
                Grid.SetColumn(image, i % cols);
                Grid.SetRow(image, i / cols);
                g.Children.Add(image);
            }

        }
    }
}

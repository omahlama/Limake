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

namespace TestRunner
{
    public partial class MainPage : UserControl, IGameDisplay
    {
        private int[] wins = new int[4];
        private int total = 0;
        private Label[] scoreControls;

        public MainPage()
        {
            InitializeComponent();
            scoreControls = new Label[] { Green, Red, Blue, Yellow };
        }

        private void Label_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            for (int j = 0; j < 4000; j++)
            {
                Game game = new Game(this, new IPlayer[] { new BasicPlayer(), new RandomPlayer(), new RandomPlayer(), new RandomPlayer() });

                Piece winner = game.Run();
                wins[(int)winner - 1]++;
                total++;

                for (int i = 0; i < 4; i++)
                {
                    scoreControls[i].Content = wins[i] + " ( " + Math.Round((double)(100.0 * wins[i]/total)) + " % )";
                }
            }
        }

        void IGameDisplay.DisplayTurn(Piece side, int turn)
        {
        }

        void IGameDisplay.DisplayRoll(int Roll)
        {
        }

        void IGameDisplay.DisplaySituation(Situation situation)
        {
        }

        void IGameDisplay.DispĺayMove(Move move)
        {
        }

        void IGameDisplay.DisplayNoMove()
        {
        }
    }
}

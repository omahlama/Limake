using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Limake;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(new ConsoleGameDisplay(), new IPlayer[] { new ConsolePlayer(), new RandomPlayer(), new RandomPlayer(), new RandomPlayer() });
            Piece winner = game.Run();
            Console.WriteLine(String.Format("Player {0} won.", winner));
            Console.ReadKey();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limake
{
    public class ConsolePlayer : IPlayer
    {
        public int SelectMove(Situation situation, Move[] moves, Piece side, int roll)
        {
            Console.WriteLine("Side: " + side);
            Console.WriteLine("Roll: " + roll);
            Console.WriteLine("Situation: ");
            Console.WriteLine(situation.ToString());
            int selectedMove = -1;
            do
            {
                Console.WriteLine("Moves");
                for (int i = 0; i < moves.Length; i++)
                {
                    Console.WriteLine(i + ": " + moves[i].ToString());
                }
                Console.WriteLine("Select move: ");
                selectedMove = Convert.ToInt32(Console.ReadLine());
            } while (selectedMove < 0 || selectedMove >= moves.Length);

            return selectedMove;
        }

        public void NoMovesAvailable(Situation situation)
        {
            Console.WriteLine("No moves available");
//            Console.WriteLine(situation.ToString());

        }
    }
}

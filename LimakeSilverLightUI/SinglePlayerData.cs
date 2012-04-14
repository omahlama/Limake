using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.Generic;
using Limake;

namespace LimakeSilverLightUI
{
    public class SinglePlayerData
    {
        private static SinglePlayerData instance;
        public static SinglePlayerData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SinglePlayerData();
                }
                return instance;
            }
        }

        private SinglePlayerData()
        {
            ReadDictionary();
        }

        public decimal GetTurnsRemainingExpectedValue(Situation s, Piece side)
        {
            return dict[GetKey(s, side)];
        }

        private Dictionary<int, decimal> dict = new Dictionary<int, decimal>();

        private void ReadDictionary()
        {
            int i = 0;
            byte[] bytes = Resource.singleplayer;
            while (i < bytes.Length)
            {
                int key = System.BitConverter.ToInt32(bytes, i);
                Int16 v = System.BitConverter.ToInt16(bytes, i + 4);

                decimal value = (decimal)v / 256;
                dict.Add(key, value);

                i += 6;
            }
        }

        private int GetKey(Situation s, Piece side)
        {
            int start = ((int)side - 1) * 4;

            byte[] arr = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                arr[i] = (byte)GetGreenEquivalent(s.pieces[start + i], side);
            }

            Array.Sort<byte>(arr);

            return (arr[0] << 24) + (arr[1] << 16) + (arr[2] << 8) + arr[3];
        }

        private Position GetGreenEquivalent(Position pos, Piece side)
        {
            int p = (int)pos;
            int delta = (int)side - 1; // Green = 1
            if (p < (int)Position.GreenGoal1)
            {
                return (Position)((28 + p - 7 * delta) % 28);
            }

            // Goal or hjembo
            return (Position)(p - 4 * delta);
        }
    }
}

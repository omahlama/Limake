using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Limake;
using System.IO;

namespace SinglePlayerSolver
{
    class SinglePlayerSolver
    {
        static void Main(string[] args)
        {
            List<Position> positionList = new List<Position>();
            positionList.AddRange(new Position[] { Position.GreenGoal4, Position.GreenGoal3, Position.GreenGoal2, Position.GreenGoal1});

            for (int p = (int)Position.Yellow6; p >= (int)Position.GreenStart; p--)
            {
                positionList.Add((Position)p);
            }
            positionList.AddRange(new Position[] { Position.GreenHome4, Position.GreenHome3, Position.GreenHome2, Position.GreenHome1 });
            Position[] positionOrder = positionList.ToArray();

            int len = positionOrder.Length;
            int homeStart = len-4;
            bool first = true;

            for (int i = 0; i < len; i++)
            {
                for (int j = Math.Max(i, 1); j < len; j++)
                {
                    if (i == j && i >= homeStart)
                        continue;

                    for (int k = Math.Max(j, 2); k < len; k++)
                    {
                        if (j == k && j >= homeStart)
                            continue;

                        for (int l = Math.Max(k,3); l < len; l++)
                        {
                            if (k == l && k >= homeStart)
                                continue;

                            var pos = getStartPositions();
                            pos[0] = positionOrder[i];
                            pos[1] = positionOrder[j];
                            pos[2] = positionOrder[k];
                            pos[3] = positionOrder[l];

                            Situation sit = new Situation(pos);
                            Situation s2 = new Situation(sit);
                            if (s2.UnfoldMultiples(Piece.Green))
                            {
                                continue;
                            }

                            int nomoves = 0;
                            decimal ev = 0;

                            if (first)
                            {
                                SaveValue(sit, 0);
                                first = false;
                                continue;
                            }

                            for (int roll = 1; roll <= 6; roll++)
                            {
                                Move[] moves = sit.GetMoves(Piece.Green, roll);
                                if (moves.Length == 0)
                                {
                                    nomoves++;
                                }
                                else
                                {
                                    decimal value = Decimal.MaxValue;
                                    foreach(Move move in moves)
                                    {
                                        Situation s = new Situation(sit);
                                        s.ApplyMove(move);
                                        value = Math.Min(value, GetValue(s));
                                    }

                                    ev += value;
                                }
                            }

                            if (sit.GetNumberOfTries(Piece.Green) == 3)
                            {
                                ev = (5 * nomoves * nomoves + (36 + 6 * nomoves + nomoves * nomoves) * ev) / (216 - nomoves * nomoves * nomoves);
                            }
                            else
                            {
                                ev = (5 + ev) / (6 - nomoves);
                            }

                            SaveValue(sit, ev);
                            Console.WriteLine(positionOrder[i] + " "
                                + positionOrder[j] + " "
                                + positionOrder[k] + " "
                                + positionOrder[l] + ": " + ev);
                        }
                    }
                }
            }

            Byte[] bytes = new Byte[6 * dict.Count];
            int offset = 0;
            foreach (KeyValuePair<int, decimal> kvp in dict)
            {
                Array.Copy(System.BitConverter.GetBytes(kvp.Key), 0, bytes, offset, 4);
                offset += 4;
                Array.Copy(System.BitConverter.GetBytes((Int16)(kvp.Value * 256)), 0, bytes, offset, 2);
                offset += 2;
            }

            File.WriteAllBytes("singleplayer.data", bytes);

            Console.ReadLine();
        }

        private static Dictionary<int, decimal> dict = new Dictionary<int,decimal>();

        private static decimal GetValue(Situation s)
        {
            return dict[GetKey(s)];
        }

        private static void SaveValue(Situation s, decimal value)
        {
            dict[GetKey(s)] = value;
        }

        private static int GetKey(Situation s)
        {
            byte[] arr = new byte[] { (byte)s.pieces[0], (byte)s.pieces[1], (byte)s.pieces[2], (byte)s.pieces[3] };
            Array.Sort<byte>(arr);

            return (arr[0] << 24) + (arr[1] << 16) + (arr[2] << 8) + arr[3];
        }

        private static Position[] getStartPositions()
        {
            return new Position[] { Position.GreenHome1, Position.GreenHome2, Position.GreenHome3, Position.GreenHome4,
                                    Position.RedHome1, Position.RedHome2,Position.RedHome3,Position.RedHome4,
                                    Position.BlueHome1,Position.BlueHome2,Position.BlueHome3,Position.BlueHome4,
                                    Position.YellowHome1,Position.YellowHome2,Position.YellowHome3,Position.YellowHome4};
        }
    }
}

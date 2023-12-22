using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AdventOfCode2023
{
    internal class Day21
    {
        private const int MapLengthX = 131;
        private const int MapLengthY = 131;
        private enum dir
        {
            north, south, west, east
        }

        private char[,] map;
        private Point start;


        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 21 !! ##################################\r\n");
            //Read and parse file
            map = new char[MapLengthX, MapLengthY];

            var inputpath = @"Inputs\day21.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                int i = 0;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().ToCharArray();
                    for (int j = 0; j < line.Length; j++)
                    {
                        map[i, j] = line[j];
                        if (line[j] == 'S') start = new Point(i, j);
                    }
                    i++;
                }
            }
            //Print(map);
            long total1 = 0;
            int steps = 0;
            var q = new Queue<Point>();
            q.Enqueue(start);
            while (steps < 64)
            {
                var nextQ = new Queue<Point>();
                while (q.Any())
                {
                    var curPos = q.Dequeue();
                    MoveAndEnqueue(nextQ, curPos, dir.north);
                    MoveAndEnqueue(nextQ, curPos, dir.south);
                    MoveAndEnqueue(nextQ, curPos, dir.east);
                    MoveAndEnqueue(nextQ, curPos, dir.west);
                }
                q = nextQ;
                steps++;
            }
            total1 = q.Count;
            Console.WriteLine($"Final result for 1st star is : {total1}");

            var progress = new AllIWantIsProgress();
            long total2 = 0;
            var alreadyVisitedOnOddSteps = new HashSet<(Point,Point)>();
            var alreadyVisitedOnEvenSteps = new HashSet<(Point,Point)>();
            steps = 0;
            q = new Queue<Point>();
            q.Enqueue(start);
            while (steps < 1000)//26_501_365)
            {
                var nextQ = new Queue<Point>();
                while (q.Any())
                {
                    var alreadyVisited = (steps % 2 == 0) ? alreadyVisitedOnEvenSteps : alreadyVisitedOnOddSteps;
                    var curPos = q.Dequeue();
                    MoveAndEnqueue2(nextQ, curPos, dir.north, alreadyVisited);
                    MoveAndEnqueue2(nextQ, curPos, dir.south, alreadyVisited);
                    MoveAndEnqueue2(nextQ, curPos, dir.east, alreadyVisited);
                    MoveAndEnqueue2(nextQ, curPos, dir.west, alreadyVisited);
                }
                q = nextQ;
                if (steps % 2 == 1)
                {
                    Console.WriteLine($"steps:{steps:000}");
                    var cnts = new int[5, 5];
                    for(int i = -2; i <= 2; i++)
                    {
                        for(int j = -2; j <= 2; j++)
                        {
                            cnts[i+2,j+2]= alreadyVisitedOnOddSteps.Count(p => p.Item2.X == i && p.Item2.Y == j);
                            Console.Write($" {cnts[i + 2, j + 2]:0000}");
                        }
                        Console.WriteLine();
                    }
                }
                steps++;
                //Console.Write($"\r{steps} ");
            }
            Console.WriteLine();
            total2 = alreadyVisitedOnOddSteps.Count;
            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 21 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private void MoveAndEnqueue(Queue<Point> nextQ, Point curPos, dir direction)
        {
            var p = Move(curPos, direction);
            if (p != null && map[p.Value.X, p.Value.Y] != '#' && !nextQ.Contains(p.Value))
            {
                nextQ.Enqueue(p.Value);
            }
        }

        private int maxScreenN = 0;
        private int maxScreenS = 0;
        private int maxScreenW = 0;
        private int maxScreenE = 0;
        private void MoveAndEnqueue2(Queue<Point> nextQ, Point curPos, dir direction, HashSet<(Point,Point)> alreadyVisited)
        {
            var p = Move(curPos, direction, false);
            if (p != null)
            {
                var pScreens= CheckScreens(p.Value);
                if (!alreadyVisited.Contains((p.Value, pScreens))){
                    var x = p.Value.X % MapLengthX;
                    if (x < 0) x += MapLengthX;
                    var y = p.Value.Y % MapLengthY;
                    if (y < 0) y += MapLengthY;
                    if (map[x, y] != '#' && !nextQ.Contains(p.Value))
                    {
                        nextQ.Enqueue(p.Value);
                        alreadyVisited.Add((p.Value,pScreens));
                    }
                }
            }
        }

        private Point CheckScreens(Point p)
        {
            int nscreenX,nscreenY;
            if (p.X >= 0)
            {
                nscreenX = p.X / MapLengthX;
                if (nscreenX > maxScreenS)
                {
                    maxScreenS = nscreenX;
                    Console.WriteLine($"\tReached screen #{maxScreenS} on South by [{p.X},{p.Y}]");
                }
            }
            else
            {
                nscreenX = (p.X / MapLengthX) - 1;
                if (-nscreenX > maxScreenN)
                {
                    maxScreenN = -nscreenX;
                    Console.WriteLine($"\tReached screen #{maxScreenN} on North by [{p.X},{p.Y}]");
                }
            }
            if (p.Y >= 0)
            {
                nscreenY = p.Y / MapLengthY;
                if (nscreenY > maxScreenE)
                {
                    maxScreenE = nscreenY;
                    Console.WriteLine($"\tReached screen #{maxScreenE} on East by [{p.X},{p.Y}]");
                }
            }
            else
            {
                nscreenY = (p.Y / MapLengthY) - 1;
                if (-nscreenY > maxScreenW)
                {
                    maxScreenW = -nscreenY;
                    Console.WriteLine($"\tReached screen #{maxScreenW} on West by [{p.X},{p.Y}]");
                }
            }
            return new Point(nscreenX,nscreenY);
        }

        private char[,] CloneMap()
        {
            var res = new char[MapLengthX, MapLengthY];
            res.ForEachIndex(p => res[p.X, p.Y] = map[p.X, p.Y]);
            return res;
        }

        private long Evaluate(char[,] map)
        {
            return map.AsEnumerable().Sum(c => c == 'O' ? 1 : 0);
        }

        private void Print(char[,] map)
        {
            Console.Clear();
            map.ForEachIndex(p =>
            {
                if (p.X > 0 && p.Y == 0) Console.WriteLine();
                if (map[p.X, p.Y] == 'O') Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(map[p.X, p.Y]);
                Console.ResetColor();
            });
        }

        private Point? Move(Point position, dir direction, bool checkBounds = true)
        {
            switch (direction)
            {
                case dir.north: position.X--; break;
                case dir.south: position.X++; break;
                case dir.east: position.Y++; break;
                case dir.west: position.Y--; break;
            }
            //Check for out of bounds
            if (checkBounds)
            {
                if (position.X < 0 || position.X > MapLengthX - 1) return null;
                if (position.Y < 0 || position.Y > MapLengthY - 1) return null;
            }
            return position;
        }
    }
}

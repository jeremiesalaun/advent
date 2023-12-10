using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day10
    {
        private char[,] map;
        public void Run()
        {
            map = new char[140, 140];
            Point start = default;
            //Read and parse file
            var inputpath = @"Inputs\day10.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                int row = 0;
                while (!sr.EndOfStream)
                {
                    var l = sr.ReadLine().ToCharArray();
                    for (int col = 0; col < l.Length; col++)
                    {
                        if (l[col] == 'S') start = new Point(row, col);
                        map[row, col] = l[col];
                    }
                    row++;
                }
            }

            long total1 = 0;
            var possiblePaths = new List<List<Point>>();
            var p = Move(start, direction.top);
            if (p != null)
            {
                var d = Connects(map[p.Value.X, p.Value.Y], direction.bottom);
                var path = Walk(p, d);
                if (path != null && path.Count > 0)
                {
                    possiblePaths.Add(path);
                }
            }
            p = Move(start, direction.bottom);
            if (p != null)
            {
                var d = Connects(map[p.Value.X, p.Value.Y], direction.top);
                var path = Walk(p, d);
                if (path != null && path.Count > 0)
                {
                    possiblePaths.Add(path);
                }
            }
            p = Move(start, direction.left);
            if (p != null)
            {
                var d = Connects(map[p.Value.X, p.Value.Y], direction.right);
                var path = Walk(p, d);
                if (path != null && path.Count > 0)
                {
                    possiblePaths.Add(path);
                }
            }
            p = Move(start, direction.right);
            if (p != null)
            {
                var d = Connects(map[p.Value.X, p.Value.Y], direction.left);
                var path = Walk(p, d);
                if (path != null && path.Count > 0)
                {
                    possiblePaths.Add(path);
                }
            }
            total1 = possiblePaths.Select(p => p.Count).Max() / 2;
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;
            var mypath = possiblePaths[0];
            var map2 = new int[140, 140];
            map2[start.X, start.Y] = 1;
            for(int i = 0; i < mypath.Count; i++)
            {
                map2[mypath[i].X, mypath[i].Y] = i+2;
            }
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (!mypath.Any(p => p.X == i && p.Y == j))
                    {
                        map[i, j] = '.';
                    }
                }
            }
            for(int i = 0;i < map2.GetLength(0)-1; i++)
            {
                var crossCounter = 0;
                for(int j=0; j < map2.GetLength(1);j++)
                {
                    if (map2[i, j] > 0)
                    {
                        var a = map2[i,j];
                        var b = map2[i+1,j];
                        if (a == b + 1)
                        {
                            crossCounter++;
                        }
                        else if (a == b - 1)
                        {
                            crossCounter--;
                        }
                    }
                    else
                    {
                        if (crossCounter > 0)
                        {
                            //Console.WriteLine($"Crosscounter at [{i},{j}] is {crossCounter}");
                            map2[i,j] = -1;
                        }
                    }
                }
            }

            total2 = CountZeroes(map2);
            Print(map, map2);
            Console.WriteLine($"Final result for 2nd star is : {total2}");
        }

        private long CountZeroes(int[,] map2)
        {
            long res = 0;
            for (int i = 0; i < map2.GetLength(0); i++)
            {
                for (int j = 0; j < map2.GetLength(1); j++)
                {
                    if (map2[i, j] == -1) res++;
                }
            }
            return res;
        }

        private void Print(char[,] map, int[,] map2)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.ForegroundColor = map2[i, j] > 0 ? (map[i, j] == 'S' ? ConsoleColor.Yellow : ConsoleColor.Red) : (map2[i, j] == 0 ? ConsoleColor.Blue : ConsoleColor.White);
                    Console.Write(map[i, j]);
                }
                Console.Write(Environment.NewLine);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private List<Point> Walk(Point? p, direction d)
        {
            var path = new List<Point>();
            while (d != direction.none)
            {
                path.Add(p.Value);
                p = Move(p.Value, d);
                if (p == null) break;
                if (map[p.Value.X, p.Value.Y] == 'S')
                {
                    Console.WriteLine($"Arrived, end of path in {path.Count} steps");
                    break;
                }
                //Console.WriteLine($"Moving direction {d} to {map[p.Value.X, p.Value.Y]}");
                d = Connects(map[p.Value.X, p.Value.Y], Next(d));
            }
            return path;
        }

        private enum direction
        {
            top, bottom, left, right, none
        }

        private direction Connects(char pipe, direction from)
        {
            switch ((pipe, from))
            {
                case ('|', direction.top): return direction.bottom;
                case ('|', direction.bottom): return direction.top;
                case ('-', direction.left): return direction.right;
                case ('-', direction.right): return direction.left;
                case ('7', direction.left): return direction.bottom;
                case ('7', direction.bottom): return direction.left;
                case ('F', direction.right): return direction.bottom;
                case ('F', direction.bottom): return direction.right;
                case ('L', direction.top): return direction.right;
                case ('L', direction.right): return direction.top;
                case ('J', direction.top): return direction.left;
                case ('J', direction.left): return direction.top;
            }
            return direction.none;
        }

        private direction Next(direction d1)
        {
            switch (d1)
            {
                case direction.top: return direction.bottom;
                case direction.bottom: return direction.top;
                case direction.left: return direction.right;
                case direction.right: return direction.left;
                default: return direction.none;
            }
        }

        private Point? Move(Point p, direction d)
        {
            Point? result = null;
            switch (d)
            {
                case direction.top: result = new Point(p.X - 1, p.Y); break;
                case direction.bottom: result = new Point(p.X + 1, p.Y); break;
                case direction.left: result = new Point(p.X, p.Y - 1); break;
                case direction.right: result = new Point(p.X, p.Y + 1); break;
            };
            if (result?.X < 0 || result?.X > 139) return null;
            if (result?.Y < 0 || result?.Y > 139) return null;
            return result;
        }
    }
}

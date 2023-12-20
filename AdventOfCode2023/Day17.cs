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
    internal class Day17
    {
        private const int MapLengthX = 141; //13;
        private const int MapLengthY = 141; //13;

        private Tile[,] map;

        private struct Step
        {
            public int X;
            public int Y;
            public dir d;
            public int cons;

            public Step(int X, int Y, dir d, int cons)
            {
                this.X = X;
                this.Y = Y;
                this.d = d;
                this.cons = cons;
            }
        }

        private class Tile
        {
            public int HeatLoss { get; set; }
            public List<dir> AlreadyVisitedFrom { get; set; } = [];
            public bool IsOut { get; set; } = false;

            public int HeatLossFromOrigin { get; set; } = int.MaxValue;
            public Point? ClosestPointToOrigin { get; set; }

            public Tile(int heatloss)
            {
                this.HeatLoss = heatloss;
            }

        }

        private enum dir
        {
            north, south, west, east, none
        }

        private class Path
        {
            public Point CurPos { get; set; }
            public dir CurDir { get; set; }
            public int HeatLoss { get; set; }
            public int Consecutive { get; set; }
            public List<(Point p, dir d, int h)> steps { get; set; } = [];

            public void Move(Point p, dir d, int heatloss)
            {
                this.CurPos = p;
                if (d != CurDir)
                {
                    Consecutive = 0;
                }
                else
                {
                    Consecutive++;
                }
                this.CurDir = d;
                this.HeatLoss += heatloss;
                this.steps.Add((p, d, heatloss));
            }

            public Path Clone()
            {
                var p = new Path()
                {
                    CurPos = this.CurPos,
                    CurDir = this.CurDir,
                    HeatLoss = this.HeatLoss,
                    Consecutive = this.Consecutive,
                };
                p.steps.AddRange(this.steps);
                return p;
            }
        }

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 17 !! ##################################\r\n");
            //Read and parse file
            map = new Tile[MapLengthX, MapLengthY];

            var inputpath = @"Inputs\day17.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                int i = 0;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().ToCharArray();
                    for (int j = 0; j < line.Length; j++)
                    {
                        map[i, j] = new Tile(int.Parse(line[j].ToString()));
                    }
                    i++;
                }
            }
            long total1 = 0;
            var moy = map.AsEnumerable().Select(t => t.HeatLoss).Average();

            var minResult = 1025;
            var dic = new Dictionary<Step, (int heatFromOrigin, double moy, List<Step> steps)>();
            var knownSteps = new Dictionary<Step, int>();
            dic.Add(new Step(0, 0, dir.none, 0), (0, 0, new List<Step>()));
            while (dic.Count > 0)
            {
                var c = dic.OrderBy(kvp => kvp.Value.moy).FirstOrDefault();
                //Console.Write($"\r[{c.Key.X},{c.Key.Y}]\t {c.Value.steps.Count} steps \t{c.Value.heatFromOrigin} heat");
                Console.Write($"\rTo depile : {dic.Count}");
                //Console.WriteLine($"[{c.Key.X},{c.Key.Y}],{c.Key.d} ETA {c.Value.estimateToTarget}");
                dic.Remove(c.Key);
                var curHeatFromOrigin = c.Value.heatFromOrigin;
                var steps = new List<Step>(c.Value.steps);
                steps.Add(c.Key);
                Point curPos = new Point(c.Key.X, c.Key.Y);
                Tile curTile = map[c.Key.X, c.Key.Y];
                dir curDir = c.Key.d;
                int consecutive = c.Key.cons;
                var possibleDirs = PossibleDirections(curPos, curDir, consecutive);
                foreach (var dir in possibleDirs)
                {
                    var p = Move(curPos, dir);
                    if (p != null)
                    {
                        var cons = (dir == curDir) ? consecutive + 1 : 0;
                        var key = new Step(p.Value.X, p.Value.Y, dir, cons);
                        var t = map[p.Value.X, p.Value.Y];
                        var curHeatLoss = curHeatFromOrigin + t.HeatLoss;
                        if (curHeatLoss < minResult)
                        {
                            if (!knownSteps.ContainsKey(key) || knownSteps[key] > curHeatLoss)
                            {
                                if (p.Value.X == MapLengthX - 1 && p.Value.Y == MapLengthY - 1)
                                {
                                    minResult = Math.Min(minResult, curHeatLoss);
                                    steps.Add(new Step(p.Value.X, p.Value.Y, dir, cons));
                                    Console.WriteLine($"Arrived to target with HeatLoss {curHeatLoss} in {steps.Count} steps");
                                    var h = curHeatLoss;
                                    for (int i = steps.Count - 1; i >= 0; i--)
                                    {
                                        if (knownSteps.ContainsKey(steps[i]))
                                        {
                                            knownSteps[steps[i]] = h;
                                        }
                                        else
                                        {
                                            knownSteps.Add(steps[i], h);
                                        }
                                        h -= map[steps[i].X, steps[i].Y].HeatLoss;
                                    }
                                    Print(steps);
                                }
                                else
                                {
                                    var estimate = EstimateHeatToTarget(p, moy);

                                    if (dic.ContainsKey(key))
                                    {
                                        if (curHeatLoss < dic[key].heatFromOrigin)
                                        {
                                            dic[key] = (curHeatLoss, curHeatLoss + estimate - steps.Count/10, steps);
                                        }
                                    }
                                    else
                                    {
                                        dic.Add(key, (curHeatLoss, curHeatLoss + estimate - steps.Count / 10, steps));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            total1 = minResult;
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;

            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 17 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private void Print(List<Step> steps)
        {
            for (int i = 0; i < MapLengthX; i++)
            {
                for (int j = 0; j < MapLengthY; j++)
                {
                    if (steps.Any(s => s.X == i && s.Y == j))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(map[i, j].HeatLoss);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        private int EstimateHeatToTarget(Point? p, double moy)
        {
            return (int)(((MapLengthX - 1 - p.Value.X) + (MapLengthY - 1 - p.Value.Y)) * moy);
        }

        private Point? Move(Point position, dir direction)
        {
            switch (direction)
            {
                case dir.north: position.X--; break;
                case dir.south: position.X++; break;
                case dir.east: position.Y++; break;
                case dir.west: position.Y--; break;
            }
            //Check for out of bounds
            if (position.X < 0 || position.X > MapLengthX - 1) return null;
            if (position.Y < 0 || position.Y > MapLengthY - 1) return null;
            return position;
        }

        private List<dir> PossibleDirections(Point position, dir direction, int consecutive)
        {
            var list = new List<dir>();
            if (consecutive < 2 && direction != dir.none) list.Add(direction);
            switch (direction)
            {
                case dir.north:
                case dir.south:
                    list.Add(dir.east); list.Add(dir.west); break;
                case dir.east:
                case dir.west:
                    list.Add(dir.north); list.Add(dir.south); break;
                case dir.none:
                    list.Add(dir.east); list.Add(dir.west); list.Add(dir.north); list.Add(dir.south); break;
            }
            if (position.X == 0) list.Remove(dir.north);
            if (position.X == MapLengthX - 1) list.Remove(dir.south);
            if (position.Y == 0) list.Remove(dir.west);
            if (position.Y == MapLengthY - 1) list.Remove(dir.east);
            return list;
        }
    }
}

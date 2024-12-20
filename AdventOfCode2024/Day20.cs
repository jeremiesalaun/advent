using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    //Replace 20 by day number
    internal class Day20
    {
        private char[,] map;
        private int[,] posMap;
        private List<Point> normalRoute;
        private Dictionary<int, HashSet<(Point s, Point e)>> shortCutsByGain;

        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 20 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day20.txt";
            map = MapHelper.LoadCharMap(path);
            var start = map.AsPointEnumerable().First(p => map.Get(p) == 'S');
            var end = map.AsPointEnumerable().First(p => map.Get(p) == 'E');

            posMap = new int[map.GetLength(0), map.GetLength(1)];
            normalRoute = [start];
            ComputeNormalRoute(start, end);
            Console.WriteLine($"Normal route has {normalRoute.Count - 1} steps");

            shortCutsByGain = new Dictionary<int, HashSet<(Point s, Point e)>>();
            foreach (var p in normalRoute)
            {
                ProcessCheat(p, 2, TEST ? 0 : 99);
            }
            if (TEST)
            {
                foreach (var k in shortCutsByGain.Keys.Order())
                {
                    Console.WriteLine($"- There are {shortCutsByGain[k].Count} cheats that save {k} picoseconds.");
                }
            }
            total1 = shortCutsByGain.Keys.Where(k => k >= 100).Sum(k => shortCutsByGain[k].Count);

            shortCutsByGain = new Dictionary<int, HashSet<(Point s, Point e)>>();
            foreach (var p in normalRoute)
            {
                ProcessCheat(p, 20, TEST ? 0 : 99);
            }
            if (TEST)
            {
                foreach (var k in shortCutsByGain.Keys.Order())
                {
                    Console.WriteLine($"- There are {shortCutsByGain[k].Count} cheats that save {k} picoseconds.");
                }
            }
            total2 = shortCutsByGain.Keys.Where(k => k >= 100).Sum(k => shortCutsByGain[k].Count);

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 20 ***********************************");
            Thread.Sleep(1000);
        }

        private void ProcessCheat(Point p,int radius,int threshold)
        {
            var posP = posMap.Get(p);
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    var dist = Math.Abs(j) + Math.Abs(i);
                    if (dist > radius) continue;
                    var x = p.Add(new Point(i, j));
                    if (map.IsOutOfBound(x) || map.Get(p) == '#') continue;
                    var posX = posMap.Get(x);
                    if (posX > posP)
                    {
                        var gain = posX - posP - dist;
                        if (gain > threshold)
                        {
                            if (!shortCutsByGain.ContainsKey(gain)) shortCutsByGain[gain] = new HashSet<(Point s, Point e)>();
                            shortCutsByGain[gain].Add((p, x));
                        }
                    }
                }
            }
        }

        private void ComputeNormalRoute(Point curPoint, Point target)
        {
            dirs nextDir = dirs.none;
            if (curPoint == target) return;
            if (IsNextStep(curPoint, dirs.north)) nextDir = dirs.north;
            else if (IsNextStep(curPoint, dirs.south)) nextDir = dirs.south;
            else if (IsNextStep(curPoint, dirs.east)) nextDir = dirs.east;
            else if (IsNextStep(curPoint, dirs.west)) nextDir = dirs.west;
            var nextPoint = curPoint.Move(nextDir);
            normalRoute.Add(nextPoint);
            posMap.Set(nextPoint, normalRoute.Count - 1);
            ComputeNormalRoute(nextPoint, target);
        }

        private bool IsNextStep(Point curPoint, dirs d)
        {
            var p = curPoint.Move(d);
            if (map.IsOutOfBound(p)) return false;
            if (map.Get(p) == '#') return false;
            if (normalRoute.Contains(p)) return false;
            return true;
        }
    }
}


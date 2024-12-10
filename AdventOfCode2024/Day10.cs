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
    internal class Day10
    {
        const bool TEST = false;

        private int[,] map;

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 10 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day10.txt";
            map = MapHelper.LoadIntMap(path);
            total1 = map.AsPointEnumerable()
                        .Where(p => map.Get(p) == 0)
                        .Select(StartHikeDistinctSummits)
                        .Sum();
            total2 = map.AsPointEnumerable()
                        .Where(p => map.Get(p) == 0)
                        .Select(StartHikeDistinctTrails)
                        .Sum();

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 10 ***********************************");
            Thread.Sleep(1000);
        }

        private int StartHikeDistinctSummits(Point p)
        {
            var summits = new HashSet<Point>();
            return GetHikeScore(p, summits);
        }

        private int StartHikeDistinctTrails(Point p)
        {
            return GetHikeScore(p, null);
        }
        private int GetHikeScore(Point p, HashSet<Point> summits)
        {
            var height = map.Get(p);
            if (height == 9)
            {
                if (summits!=null && summits.Contains(p)) return 0;
                if(summits!=null) summits.Add(p);
                //Console.WriteLine($"Reached summit at {p}");
                return 1;
            }
            else if (height == 0)
            {
                //Console.WriteLine($"Starting hike at {p}");
            }
            int res = 0;
            res += FindHikeTrail(p, dirs.north, summits);
            res += FindHikeTrail(p, dirs.east, summits);
            res += FindHikeTrail(p, dirs.west, summits);
            res += FindHikeTrail(p, dirs.south, summits);
            return res;
        }

        private int FindHikeTrail(Point p, dirs d, HashSet<Point> summits)
        {
            var curHeight = map.Get(p);
            var p1 = p.Move(d);
            if (map.IsOutOfBound(p1)) return 0;
            if (map.Get(p1) != curHeight + 1) return 0;
            //Console.Write($"{d},");
            return GetHikeScore(p1, summits);
        }
    }
}


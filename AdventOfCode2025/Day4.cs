using AdventOfCode2025.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2025
{
    //Replace 4 by day number
    internal class Day4
    {
        const bool TEST = false;
        char[,] map;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 4  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day4.txt";
            map = MapHelper.LoadCharMap(path);
            map.ForEachIndex(p =>
            {
                if (map.Get(p)=='@' && countAdjacent(p) < 4)
                {
                    total1++;
                }
            });

            while (true)
            {
                Console.Write(".");
                int removed = 0;
                map.ForEachIndex(p =>
                {
                    if (map.Get(p) == '@' && countAdjacent(p) < 4)
                    {
                        map.Set(p, 'x');
                        removed++;
                    }
                });
                total2 += removed;
                if (removed == 0) break;
            }
            Console.WriteLine();


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 4 ***********************************");
            Thread.Sleep(1000);
        }

        private int countAdjacent(Point p)
        {
            int c = 0;
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    var q = new Point(p.X + i,p.Y+j);
                    if (map.IsOutOfBound(q)) continue;
                    if (map.Get(q) == '@') c++;
                }
            }
            return c;
        }
    }
}


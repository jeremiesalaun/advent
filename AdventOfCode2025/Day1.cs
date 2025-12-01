using AdventOfCode2025.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2025
{
    internal class Day1
    {

        private class turn
        {
            public int dir = 1;
            public int distance = 0;
            public int move(int pos)
            {
                return (pos + dir * distance) % 100;
            }

            public int move2(int pos)
            {
                return (pos + dir * distance);
            }
        }

        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 1 ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day1.txt";
            var instructions = File.ReadAllLines(path).Select(s => new turn() { dir = (s[0] == 'L' ? -1 : 1), distance=(int.Parse(s.Substring(1))) }).ToList();

            var curpos = 50;
            foreach(var i in instructions)
            {
                curpos = i.move(curpos);
                if (curpos == 0) total1++;
            }

            curpos = 50;
            foreach (var i in instructions)
            {
                var newpos = i.move2(curpos);
                int x;
                if (newpos > curpos)
                {
                    x  = (newpos / 100) - (curpos/100);
                    if (newpos > 0 && curpos < 0) x++;
                }
                else
                {
                    x = (curpos / 100) - (newpos / 100);
                    if (curpos > 0 && newpos < 0) x++;
                }
                if (newpos == 0) x++;
                Console.WriteLine($"Going from {curpos} to {newpos} : passing {x} times by 0");
                total2 += x;
                curpos = newpos % 100;
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 1 ***********************************");
            Thread.Sleep(1000);
        }

    }
}


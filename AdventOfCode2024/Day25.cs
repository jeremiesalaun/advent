using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Day25
    {

        private List<int[]> Locks = new List<int[]>();
        private List<int[]> Keys = new List<int[]>();

        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 25 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day25.txt";
            var lines = File.ReadAllLines(path);
            foreach(var input in lines.Chunk(8))
            {
                if (input[0][0] == '#')
                {
                    ProcessLock(input);
                }
                else
                {
                    ProcessKey(input);
                }
            }
            foreach(var l in Locks)
            {
                foreach(var k in Keys)
                {
                    if (Match(k, l))
                    {
                        total1++;
                    }
                }
            }


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 25 ***********************************");
            Thread.Sleep(1000);
        }

        private bool Match(int[] k, int[] l)
        {
            for (int i = 0; i < 5; i++)
            {
                if (k[i] + l[i] > 5) return false;
            }
            return true;
        }

        private void ProcessLock(string[] input)
        {
            var vals = new int[5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (input[j][i] == '.')
                    {
                        vals[i] = j-1;
                        break;
                    }
                }
            }
            Locks.Add(vals);
        }

        private void ProcessKey(string[] input)
        {
            var vals = new int[5];
            for (int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 7; j++)
                {
                    if (input[j][i] == '#')
                    {
                        vals[i] = 6 - j;
                        break;
                    }
                }
            }
            Keys.Add(vals);
        }
    }
}


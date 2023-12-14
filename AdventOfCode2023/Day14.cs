using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day14
    {
        private char[,] map = new char[100, 100];

        public void Run()
        {
            //Read and parse file
            var inputpath = @"Inputs\day14.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                var i = 0;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().ToCharArray();
                    for(int j = 0; j < line.Length; j++)
                    {
                        map[i,j] = line[j];
                    }
                    i++;
                }
            }

            MoveNS(1);
            long total1 = Evaluate();

            Console.WriteLine($"Final result for 1st star is : {total1}");
            var dico = new Dictionary<string, int>();
            var foundPeriod = false;
            var nbCycles = 1_000_000_000;
            for (int k=0; k<nbCycles; k++)
            {
                MoveNS(1); //North
                MoveEW(1); //West
                MoveNS(-1); //South
                MoveEW(-1); //East
                if (!foundPeriod)
                {
                    var key = MakeKey();
                    if (!dico.ContainsKey(key))
                    {
                        dico.Add(key, k);
                    }
                    else
                    {
                        foundPeriod= true;
                        var periode = k - dico[key];
                        Console.WriteLine($"MATCH between {k} and {dico[key]}, period is {periode} !!!");               
                        k = nbCycles - (nbCycles - k) % periode;
                        Console.WriteLine($"WARP to {k}");
                    }
                }
            }
            long total2 = Evaluate();
            Console.WriteLine($"Final result for 2nd star is : {total2}");
        }

        private string MakeKey()
        {
            var sb=new StringBuilder();
            for(int i=0; i < 100; i++)
            {
                for(int j=0; j < 100; j++)
                {
                    sb.Append(map[i,j]);
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private long Evaluate()
        {
            long total = 0;
            var lineScore = 1;
            for(int i = 99; i >= 0; i--)
            {
                var nbOs = 0;
                for(int j = 0; j < 100; j++)
                {
                    if (map[i, j] == 'O') nbOs++;
                }
                total += lineScore * nbOs;
                lineScore++;
            }
            return total;
        }

        private void MoveNS(int incr)
        {
            //Scan by column
            for (int j = 0; j < 100; j++)
            {
                int lowestEmptyRow = -1;
                int start = incr > 0 ? 0 : 99;
                int stop = incr > 0 ? 100 : -1;
                for (int i = start; Math.Abs(stop-i) >0 ; i+=incr)
                {
                    switch (map[i, j])
                    {
                        case '.':
                            if (lowestEmptyRow < 0) lowestEmptyRow = i;
                            break;
                        case '#':
                            lowestEmptyRow = -1; break;
                        case 'O':
                            if (lowestEmptyRow >= 0)
                            {
                                map[lowestEmptyRow, j] = 'O';
                                map[i, j] = '.';
                                lowestEmptyRow += incr;
                            }
                            break;
                    }
                }
            }
        }

        private void MoveEW(int incr)
        {
            //Scan by row
            for (int i = 0; i < 100; i++)
            {
                int lowestEmptyCol = -1;
                int start = incr > 0 ? 0 : 99;
                int stop = incr > 0 ? 100 : -1;
                for (int j = start; Math.Abs(stop - j) > 0; j += incr)
                {
                    switch (map[i, j])
                    {
                        case '.':
                            if (lowestEmptyCol < 0) lowestEmptyCol = j;
                            break;
                        case '#':
                            lowestEmptyCol = -1; break;
                        case 'O':
                            if (lowestEmptyCol >= 0)
                            {
                                map[i, lowestEmptyCol] = 'O';
                                map[i, j] = '.';
                                lowestEmptyCol += incr;
                            }
                            break;
                    }
                }
            }
        }
    }
}

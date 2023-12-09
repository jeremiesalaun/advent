using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day9
    {
        public void Run()
        {
            var series = new List<List<long>>();
            //Read and parse file
            var path = @"Inputs\day9.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    var l = ParseLine(sr.ReadLine());
                    if(l != null) {  series.Add(l); }
                }
            }
            long total1 = 0;
            long total2 = 0;
            for(int i = 0; i < series.Count; i++)
            {
                Console.WriteLine();
                var (p,n) = FindNextAndPrevious(series[i]);
                Console.WriteLine($"{p} -> [{string.Join(',', series[i])}] -> {n}");
                total2 += p;
                total1 += n;
            }
            Console.WriteLine($"Final result for 1st star is : {total1}");
            Console.WriteLine($"Final result for 2nd star is : {total2}");
        }

        private List<long> ParseLine(string line)
        {
            if (line == null) return null;
            return line.Split(' ').Select(long.Parse).ToList();
        }

        private (long p,long n) FindNextAndPrevious(List<long> series)
        {
            if (series.TrueForAll(x => x == 0))
            {
                Console.WriteLine($"\tAnswer (0,0)");
                return (0,0);
            }
            else
            {
                List<long> deltas = GetDeltas(series);
                var (d0,dn) = FindNextAndPrevious(deltas);
                Console.WriteLine($"\tAnswer ({series[0] - d0},{series[series.Count - 1] + dn})");
                return (series[0]-d0,series[series.Count - 1] + dn);
            }
        }

        private static List<long> GetDeltas(List<long> series)
        {
            var deltas = new List<long>();
            for (int i = 0; i < series.Count - 1; i++)
            {
                deltas.Add(series[i + 1] - series[i]);
            }
            Console.WriteLine($"\tDeltas {string.Join(',', deltas)}");
            return deltas;
        }
    }
}

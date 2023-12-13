using System;
using System.Collections;
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
    internal class Day12
    {
        private Dictionary<string, long> cache = new Dictionary<string, long>();
        private class row
        {
            public char[] springs { get; set; }
            public List<int> brokenGroups { get; set; } = [];
            public void Unfold(int times)
            {
                char[] c = new char[springs.Length * times + (times - 1)];
                var b = new List<int>();
                var u = new List<string>();
                for (int i = 0; i < times; i++)
                {
                    springs.CopyTo(c, i * springs.Length + i);
                    if (i < times - 1)
                    {
                        c[springs.Length * (i + 1) + i] = '?';
                    }
                    b.AddRange(brokenGroups);
                }
                this.springs = c;
                this.brokenGroups = b;
            }
        }
        public void Run()
        {
            var rows = new List<row>();
            //Read and parse file
            var inputpath = @"Inputs\day12.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                while (!sr.EndOfStream)
                {
                    rows.Add(ParseLine(sr.ReadLine()));
                }
            }


            long total1 = 0;
            foreach (var row in rows)
            {
                var r = Calculate(row.springs, row.brokenGroups);
                total1 += r;
            }

            Console.WriteLine($"Final result for 1st star is : {total1}");

            rows.ForEach(r => r.Unfold(5));
            long total2 = 0;
            foreach (var row in rows)
            {
                var r = Calculate(row.springs, row.brokenGroups);
                //Console.WriteLine($"{new string(row.springs)} {string.Join(',', row.brokenGroups)}: {r}");
                total2 += r;
            }
            Console.WriteLine($"Final result for 2nd star is : {total2}");
        }

        private long Calculate(char[] springs, List<int> brokenGroups)
        {
            var cacheKey = $"{new string(springs)}_{string.Join('|', brokenGroups)}";
            if (!cache.ContainsKey(cacheKey))
            {
                cache.Add(cacheKey, InternalCalculate(springs, brokenGroups));
            }
            return cache[cacheKey];
        }

        private long InternalCalculate(char[] springs, List<int> brokenGroups)
        {
            if (brokenGroups.Count == 0)
            {
                return springs.Contains('#') ? 0 : 1;
            }
            if (springs.Length == 0)
            {
                return 0;
            }
            switch (springs[0])
            {
                case '.':
                    var i = 0; //Eat all the dots
                    while (i < springs.Length && springs[i] == '.')
                    {
                        i++;
                    }
                    return Calculate(springs[i..], brokenGroups);
                case '?':
                    var sp = springs[..];
                    sp[0] = '#';
                    var r = Calculate(sp, brokenGroups); //Evaluate if we put a #
                    return r + Calculate(springs[1..], brokenGroups); //Consider we put a . and skip to next position
                case '#':
                    var g = brokenGroups[0];
                    if (springs.Length < g)
                    {
                        //Not enough chars left for this group
                        return 0;
                    }
                    var n = 0;
                    while (n < g && (springs[n] == '#' || springs[n] == '?'))
                    {
                        n++;
                    }
                    if (n == g) //We completed the group so let's go to the next
                    {
                        if (n < springs.Length) //Next 2 conditions only if we are not already at the end
                        {
                            //The group MUST be followed by a '.'
                            if (springs[n] == '#') return 0;
                            //The next char must be ., so we skip it
                            n++;
                        }
                        return Calculate(springs[n..], brokenGroups[1..]);
                    }
                    else if (n < g) //We reached end of group before length of group, so we are KO
                    {
                        return 0;
                    }
                    break;
            }
            return 0;
        }

        private row ParseLine(string line)
        {
            var row = new row();
            row.springs = line.Split(" ")[0].ToCharArray();
            row.brokenGroups.AddRange(line.Split(" ")[1].Split(",").Select(int.Parse));
            return row;
        }

    }
}

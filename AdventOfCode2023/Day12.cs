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

        private class row { 
            public char[] springs { get; set; }
            public List<int> brokenGroups { get; set; } = [];
            public void Unfold(int times)
            {
                char[] c = new char[springs.Length * times + (times - 1)];
                var b = new List<int>();
                var u = new List<string>();
                for(int i = 0; i < times; i++)
                {
                    springs.CopyTo(c, i * springs.Length+i);
                    if (i < times - 1)
                    {
                        c[springs.Length * (i+1) + i] = '?';
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
            total1 = rows.AsParallel()
                        .WithDegreeOfParallelism(8)
                        .Sum(row =>
                        {
                            //Console.WriteLine($"\t{new string(row.springs)} {string.Join(',',row.brokenGroups)}");
                            var re = BuildRegex(row.brokenGroups, 0);
                            var hash = new Dictionary<(int pos, int rem,int remg), long>();
                            var remainingHashes = row.brokenGroups.Sum()-row.springs.Count(c=>c=='#');
                            var line = new char[row.springs.Length];
                            row.springs.CopyTo(line,0);
                            var l = CheckPossible(re, line,0,remainingHashes, row.brokenGroups.Count, hash);
                            return l;
                        });            

            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;
            rows.ForEach(r => r.Unfold(5));

            total2 = rows.AsParallel()
                        .WithDegreeOfParallelism(8)
                        .Sum(row =>
                        {
                            var re = BuildRegex(row.brokenGroups, 0);
                            var hash = new Dictionary<(int pos, int rem, int remg), long>();
                            var remainingHashes = row.brokenGroups.Sum() - row.springs.Count(c => c == '#');
                            var line = new char[row.springs.Length];
                            row.springs.CopyTo(line, 0);
                            var l = CheckPossible(re, line, 0, remainingHashes, row.brokenGroups.Count, hash);
                            Console.WriteLine($"{l} possibilities for {new string(line)}");
                            return l;
                        });
            Console.WriteLine($"Final result for 2nd star is : {total2}");
        }

        private long CheckPossible(Regex re, char[] line,int startIndex,int remainingHashes,int remainingGroups, Dictionary<(int pos,int rem,int remg), long> hash,bool disableCache=false)
        {
            int i = startIndex;
            //if (remainingHashes == 0)
            //{
            //    for (int j = i; j < line.Length; j++)
            //    {
            //        if (line[j] == '?')
            //        {
            //            line[j] = '.';
            //        }
            //    }
            //    i = line.Length;
            //}
            while (i<line.Length && line[i]!='?')
            {
                if (i>0 && line[i]=='.' && line[i - 1] == '#')
                {
                    remainingGroups--;
                }
                i++;
            }
            if (i==line.Length)
            {
                //Console.WriteLine($"\t{new string(line)}");
                return 1;
            }
            else if (!disableCache && hash.ContainsKey((i,remainingHashes,remainingGroups)))
            {
                //Console.WriteLine($"\t{new string(line)},position {i}, {remainingHashes}# left, {remainingGroups}g left : already evaluated : {hash[(i, remainingHashes, remainingGroups)]} possibilities");
                return hash[(i, remainingHashes,remainingGroups)];
            }
            else
            {
                long result = 0;
                var line1 = new char[line.Length];
                line.CopyTo(line1, 0);
                if (remainingHashes > 0)
                {
                    line1[i] = '#';                    
                    if (TestHypo(re, line1))
                    {
                        result += CheckPossible(re, line1,i+1,remainingHashes-1,remainingGroups, hash,disableCache);
                    }
                }
                line1[i] = '.';
                if (TestHypo(re, line1))
                {
                    var remg = remainingGroups;
                    if (i>0 && line1[i - 1] == '#')
                    {
                        remg--;
                    }
                    result+= CheckPossible(re, line1,i+1,remainingHashes,remg,hash,disableCache);
                }
                if(!disableCache) hash[(i,remainingHashes,remainingGroups)] = result;
                return result;
            }
        }

        private bool TestHypo(Regex re, char[] line)
        {
            var l = new string(line);
            var r= re.IsMatch(l);
            //Console.WriteLine($"Match for {l} is {r}");
            return r;
        }

        private Regex BuildRegex(List<int> brokenGroups, int startingGroup)
        {
            var sb = new StringBuilder();
            sb.Append(@"^[\.\?]*");
            for(int i=startingGroup; i<brokenGroups.Count ; i++)
            {
                sb.Append(@"[#\?]{"+ brokenGroups[i] +"}");
                if(i< brokenGroups.Count - 1)
                {
                    sb.Append(@"[\.\?]+");
                }
            }
            sb.Append(@"[\.\?]*$");
            return new Regex(sb.ToString());
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

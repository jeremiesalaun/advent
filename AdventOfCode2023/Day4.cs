using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day4
    {
        private class card
        {
            public int CardNum {  get; set; }
            public int Copies { get; set; } = 1;
            public List<int> WinningNumbers { get; set; } = [];
            public List<int> Numbers { get; set; } = [];

            public int CountPoints()
            {
                return Numbers.Count(n=>WinningNumbers.Contains(n));
            }

            public int GetScore()
            {
                var score = 0;
                foreach (int n in Numbers)
                {
                    if (WinningNumbers.Contains(n))
                    {
                        if (score == 0)
                        {
                            score = 1;
                        }
                        else
                        {
                            score *= 2;
                        }
                    }
                }
                return score;
            }
        }
        public void Run()
        {
            var cards = new List<card>();
            //Read and parse file
            var path = @"Inputs\day4.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    cards.Add(ParseLine(sr.ReadLine()));
                }
            }
            int total1 = cards.Sum(c=>c.GetScore());
            Console.WriteLine($"Final result for 1st star is : {total1}");

            for (int i=0; i<cards.Count; i++)
            {
                var card = cards[i];
                for(int j=1;j<=card.CountPoints(); j++)
                {
                    cards[i + j].Copies+=card.Copies;
                }
            }
            int total2 = cards.Sum(c => c.Copies);
            //Print out total result
            Console.WriteLine($"Final result for 2nd start is : {total2}");
        }

        private card ParseLine(string? line)
        {
            if (line == null) return null;
            var reline = new Regex(@"Card\s+(?<cardnum>\d+):(\s*(?<winnum>\d+)\s*)+\|(\s*(?<num>\d+)\s*)+");
            var result = new card();
            var m = reline.Match(line);
            if (m.Success)
            {
                result.CardNum = int.Parse(m.Groups["cardnum"].Value);
                result.WinningNumbers = m.Groups["winnum"].Captures.Select(c => int.Parse(c.Value)).ToList();
                result.Numbers = m.Groups["num"].Captures.Select(c => int.Parse(c.Value)).ToList();
            }
            return result;
        }
    }
}

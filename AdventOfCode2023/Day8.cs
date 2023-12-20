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
    internal class Day8
    {
        private Dictionary<string,(string left, string right,bool win)> navigation = [];
        private bool[] instructions = [];

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 8 !! ##################################\r\n");
            //Read and parse file
            var path = @"Inputs\day8.txt";
            using (var sr = new StreamReader(path, true))
            {
                var s = sr.ReadLine();
                instructions = s.ToCharArray().Select(c => c == 'L').ToArray();
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    var r = ParseLine(sr.ReadLine());
                    if (r.point != null)
                    {
                        navigation.Add(r.point, (r.left, r.right, r.win));
                    }
                }
            }
            int total1 = 0;
            int curInstruction = 0;
            var curPos = "AAA";
            while (curPos != "ZZZ")
            {
                var n = navigation[curPos];
                var i = instructions[curInstruction];
                curPos = i ? n.left : n.right;
                total1++;
                curInstruction++;
                if (curInstruction == instructions.Length) curInstruction = 0;
            }
            Console.WriteLine($"Final result for 1st star is : {total1}");

            var points = navigation.Keys.ToArray();
            var lefts = new Span<int>(navigation.Values.Select(n => Array.IndexOf(points, n.left)).ToArray());
            var rights = new Span<int>(navigation.Values.Select(n => Array.IndexOf(points, n.right)).ToArray());
            var wins = new Span<bool>(navigation.Values.Select(n => n.win).ToArray());
            var initPositions = navigation.Keys.Where(p => p.EndsWith('A')).Select(p => Array.IndexOf(points, p)).ToArray();
            var curPositions = new Span<int>((int[])initPositions.Clone());
            var distances = new long[initPositions.Length];

            for (int i = 0; i < initPositions.Length; i++)
            {
                long loops = 0;
                while (true)
                {
                    for (int j = 0; j < instructions.Length; j++)
                    {
                        curPositions[i] = instructions[j] ? lefts[curPositions[i]] : rights[curPositions[i]];
                        if (wins[curPositions[i]])
                        {
                            distances[i] = loops * instructions.Length + j + 1;
                            break;
                        }
                    }
                    if (distances[i] > 0) { break; }
                    loops++;
                }
                Console.WriteLine($"Position {points[initPositions[i]]} goes to {points[curPositions[i]]} in {distances[i]} steps");
            }

            long total2 = LcmHelper.CalculateLCM(distances);

            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 8 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private (string point,string left, string right, bool win) ParseLine(string line)
        {
            var reline = new Regex(@"(?<point>\w{3}) = \((?<left>\w{3}), (?<right>\w{3})\)");
            var m = reline.Match(line);
            if (m.Success)
            {
                return (m.Groups["point"].Value, m.Groups["left"].Value, m.Groups["right"].Value, m.Groups["point"].Value.EndsWith('Z'));
            }
            return (null, null, null,false);
        }

 

    }
}

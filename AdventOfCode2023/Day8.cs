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
            //Read and parse file
            var path = @"Inputs\day8.txt";
            using (var sr = new StreamReader(path, true))
            {
                var s = sr.ReadLine();
                instructions = s.ToCharArray().Select(c=>c=='L').ToArray();
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    var r = ParseLine(sr.ReadLine());
                    if (r.point != null)
                    {
                        navigation.Add(r.point, (r.left, r.right,r.win));
                    }
                }
            }
            int total1 = 0;
            int curInstruction = 0;
            var curPos = "AAA";
            while(curPos != "ZZZ")
            {
                var n = navigation[curPos];
                var i = instructions[curInstruction];
                curPos = i ? n.left : n.right;
                total1++;
                curInstruction++;
                if(curInstruction==instructions.Length) curInstruction= 0;
            }
            Console.WriteLine($"Final result for 1st star is : {total1}");

            var sw = Stopwatch.StartNew();
            long total2 = 0;
            curInstruction = 0;
            var points = navigation.Keys.ToArray();
            var lefts = new Span<int>(navigation.Values.Select(n=> Array.IndexOf(points,n.left)).ToArray());
            var rights = new Span<int>(navigation.Values.Select(n => Array.IndexOf(points, n.right)).ToArray());
            var wins = new Span<bool>(navigation.Values.Select(n => n.win).ToArray());
            var curPositions = new Span<int>(navigation.Keys.Where(p => p.EndsWith('A')).Select(p=>Array.IndexOf(points,p)).ToArray());

            while (true)
            {
                var insIsLeft = instructions[curInstruction];
                //for(int i = 0; i < curPositions.Length; i++)
                //{
                //    var j = curPositions[i];
                //    if (insIsLeft)
                //    {
                //        curPositions[i] = lefts[j];
                //    }
                //    else
                //    {
                //        curPositions[i] = rights[j];
                //    }
                //    win &= wins[curPositions[i]];
                //}
                if (insIsLeft)
                {
                    curPositions[0] = lefts[curPositions[0]];
                    curPositions[1] = lefts[curPositions[1]];
                    curPositions[2] = lefts[curPositions[2]];
                    curPositions[3] = lefts[curPositions[3]];
                    curPositions[4] = lefts[curPositions[4]];
                    curPositions[5] = lefts[curPositions[5]];
                } else
                {
                    curPositions[0] = rights[curPositions[0]];
                    curPositions[1] = rights[curPositions[1]];
                    curPositions[2] = rights[curPositions[2]];
                    curPositions[3] = rights[curPositions[3]];
                    curPositions[4] = rights[curPositions[4]];
                    curPositions[5] = rights[curPositions[5]];
                }
                bool win = wins[curPositions[0]] && wins[curPositions[1]] && wins[curPositions[2]] && wins[curPositions[3]] && wins[curPositions[4]] && wins[curPositions[5]];


                total2++;
                if (total2 % 1_000_000_000 == 0) Console.WriteLine($"le milliard after {sw.Elapsed.TotalSeconds}s !");
                curInstruction++;
                if (curInstruction == instructions.Length) curInstruction = 0;
                if (win) break;
            }
            sw.Stop();

            //var curPositions = navigation.Keys.Where(p=>p.EndsWith('A')).ToList();
            //while (true)
            //{
            //    var i = instructions[curInstruction];

            //    curPositions = curPositions.Select(p => {
            //        var v = navigation[p];
            //        return (i == 'L') ? v.left : v.right;
            //    }).ToList();
            //    total2++;
            //    curInstruction++;
            //    if (curInstruction == instructions.Count) curInstruction = 0;
            //    if (curPositions.TrueForAll(n => n.EndsWith('Z'))) break;
            //}

            Console.WriteLine($"Final result for 2nd start is : {total2} in {sw.Elapsed.Seconds}n");
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

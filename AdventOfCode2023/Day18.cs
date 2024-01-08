using Rubjerg.Graphviz;
using System.Drawing;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal class Day18
    {
        private enum dir
        {
            up, down, left, right
        }

        private class instruction
        {
            public dir direction { get; set; }
            public int length { get; set; }
            public string color { get; set; }

            internal Point Move(Point pos)
            {
                switch (this.direction)
                {
                    case dir.up: pos.X -= length; break;
                    case dir.down: pos.X += length; break;
                    case dir.left: pos.Y -= length; break;
                    case dir.right: pos.Y += length; break;
                }
                return pos;
            }
        }

        private List<instruction> instructions = [];
        private List<instruction> instructions2 = [];

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 18 !! ##################################\r\n");
            //Read and parse file
            var inputpath = @"Inputs\day18.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    instructions.Add(ParseLine(line));
                    instructions2.Add(ParseLine2(line));
                }
            }
            long total1 = Calculate(instructions);
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = Calculate(instructions2);

            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 18 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private long Calculate(List<instruction> instructions)
        {
            //Using Pick's theorem
            long sum = 0;
            Point curPos = new Point(0, 0);
            long perimeter = 0;
            for (int i = 0; i < instructions.Count; i++)
            {
                var nextPos = instructions[i].Move(curPos);
                perimeter += Math.Abs((nextPos.Y - curPos.Y) + (nextPos.X - curPos.X));
                sum += ((long)curPos.X * nextPos.Y) - ((long)curPos.Y * nextPos.X);
                curPos = nextPos;
            }
            return perimeter/2 + Math.Abs(sum) / 2 +1;
        }

        private instruction ParseLine(string line)
        {
            var re = new Regex(@"(?<dir>[UDLR]) (?<length>\d+) \(#(?<color>[0-9a-f]{6})\)");
            var match = re.Match(line);
            if (match.Success)
            {
                var result = new instruction();
                switch (match.Groups["dir"].Value)
                {
                    case "U": result.direction = dir.up; break;
                    case "D": result.direction = dir.down; break;
                    case "L": result.direction = dir.left; break;
                    case "R": result.direction = dir.right; break;
                }
                result.length = int.Parse(match.Groups["length"].Value);
                result.color = match.Groups["color"].Value;
                return result;
            }
            return null;
        }

        private instruction ParseLine2(string line)
        {
            var re = new Regex(@"[UDLR] \d+ \(#(?<length>[0-9a-f]{5})(?<dir>[0-9a-f])\)");
            var match = re.Match(line);
            if (match.Success)
            {
                var result = new instruction();
                switch (match.Groups["dir"].Value)
                {
                    case "3": result.direction = dir.up; break;
                    case "1": result.direction = dir.down; break;
                    case "2": result.direction = dir.left; break;
                    case "0": result.direction = dir.right; break;
                }
                result.length = int.Parse(match.Groups["length"].Value,NumberStyles.AllowHexSpecifier);
                return result;
            }
            return null;
        }
    }
}

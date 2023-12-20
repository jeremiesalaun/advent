using System.Drawing;
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

        private enum turn
        {
            none, babord, tribord
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

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 18 !! ##################################\r\n");
            //Read and parse file
            var inputpath = @"Inputs\day18.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                while (!sr.EndOfStream)
                {
                    instructions.Add(ParseLine(sr.ReadLine()));
                }
            }
            long total1 = 0;
            total1 = Algo2();
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;

            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 18 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private long Algo2()
        {
            long result = 0;
            var turns = new List<(turn t, int length)>();
            //Convert instructions to a list of turns
            for (int i = 0; i < instructions.Count - 1; i++)
            {
                turn t = FindTurn(instructions[i].direction, instructions[i + 1].direction);
                turns.Add((t, instructions[i].length));
            }
            turns.Add((turn.none, instructions[instructions.Count - 1].length));
            //Determine if the inside of the loop is at babord or tribord
            var cntB = turns.Count(t => t.t == turn.babord);
            var cntT = turns.Count(t => t.t == turn.tribord);
            var inside = (cntB > cntT) ? turn.babord : turn.tribord;

            while (turns.Count > 4)
            {
                //Print(turns, instructions[0].direction);
                CheckLoop(turns, instructions[0].direction);
                for (int i = 1; i < turns.Count - 3; i++)
                {
                    if (turns[i].t != inside) continue;
                    var j = i;
                    while (turns[j].t == inside && j < turns.Count - 2)
                    {
                        j++;
                    }
                    if (j - i > 4)
                    {
                        Console.WriteLine("probleme");
                    }
                    //Each time we have two or three consecutive inside turns, we can cut out a rectangle and simplify the remaining course
                    switch (j - i)
                    {
                        case 3:
                            if (turns[i + 1].length > turns[i + 3].length) { 
                                i++;
                                result += Manage2Turns(turns, inside, i);
                            }
                            else
                            {
                                result += Manage3Turns(turns, inside, i);
                            }
                            break;
                        case 2:
                            result += Manage2Turns(turns, inside, i);
                            break;
                        default: continue;
                    }
                    break;
                }
            }
            result += ((turns[0].length + 1) * (turns[1].length + 1));
            //Result is the sum of the area of all the rectangles.
            return result;
        }

        private static long Manage3Turns(List<(turn t, int length)> turns, turn inside, int i)
        {
            var area = (turns[i + 1].length + 1) * Math.Min(turns[i].length, turns[i + 2].length);
            Console.WriteLine($"Removing area {area} (3 corners)");
            if (area == 24) { Console.WriteLine("stop"); }
            if (turns[i].length > turns[i + 2].length)
            {
                turns[i] = (inside, turns[i].length - turns[i + 2].length);
                turns[i + 1] = (turns[i + 3].t, turns[i + 1].length + turns[i + 3].length);
                turns.RemoveAt(i + 3);
                turns.RemoveAt(i + 2);
            }
            else if (turns[i].length < turns[i + 2].length)
            {
                turns[i - 1] = (inside, turns[i - 1].length + turns[i + 1].length);
                turns[i + 2] = (turns[i + 2].t, turns[i + 2].length - turns[i].length);
                turns.RemoveAt(i + 1);
                turns.RemoveAt(i);
            }
            else //equals
            {
                Console.WriteLine("Not possible");
                //turns[i - 1] = (inside, turns[i - 1].length + turns[i + 1].length - turns[i + 3].length);
                //turns.RemoveAt(i + 3);
                //turns.RemoveAt(i + 2);
                //turns.RemoveAt(i + 1);
                //turns.RemoveAt(i);
            }

            return area;
        }

        private static long Manage2Turns(List<(turn t, int length)> turns, turn inside, int i)
        {
            var area = (turns[i + 1].length + 1) * Math.Min(turns[i].length, turns[i + 2].length);
            Console.WriteLine($"Removing area {area} (2 corners)");
            if (turns[i].length > turns[i+2].length)
            {
                turns[i] = (inside, turns[i].length - turns[i + 2].length);
                turns[i+1] = (turns[i + 3].t, turns[i + 3].length + turns[i + 1].length);
                turns.RemoveAt(i + 3);
                turns.RemoveAt(i + 2);
            }
            else if (turns[i].length < turns[i + 2].length)
            {
                turns[i - 1] = (inside, turns[i - 1].length + turns[i + 1].length);
                turns[i + 2] = (turns[i + 2].t, turns[i + 2].length - turns[i].length);
                turns.RemoveAt(i + 1);
                turns.RemoveAt(i);
            }
            else //equals
            {
                turns[i - 1] = (turns[i + 3].t, turns[i - 1].length + turns[i + 1].length + turns[i + 3].length);
                turns.RemoveAt(i + 3);
                turns.RemoveAt(i + 2);
                turns.RemoveAt(i + 1);
                turns.RemoveAt(i);
            }

            return area;
        }

        private turn FindTurn(dir direction1, dir direction2)
        {
            switch ((direction1, direction2))
            {
                case (dir.up, dir.left):
                case (dir.left, dir.down):
                case (dir.down, dir.right):
                case (dir.right, dir.up):
                    return turn.babord;
                case (dir.up, dir.right):
                case (dir.right, dir.down):
                case (dir.down, dir.left):
                case (dir.left, dir.up):
                    return turn.tribord;
            }
            return turn.none;
        }

        private dir NewDirection(dir curDir, turn t)
        {
            if (t == turn.none) return curDir;
            switch (curDir)
            {
                case dir.up: return t == turn.babord ? dir.left : dir.right;
                case dir.down: return t == turn.babord ? dir.right : dir.left;
                case dir.left: return t == turn.babord ? dir.down : dir.up;
                case dir.right: return t == turn.babord ? dir.up : dir.down;
            }
            return curDir;
        }

        private long Algo1(List<instruction> instructions)
        {
            long total1;
            Point curPos = new Point(0, 0);
            int minX = 0, minY = 0, maxX = 0, maxY = 0;
            foreach (var instruction in instructions)
            {
                curPos = instruction.Move(curPos);
                minX = Math.Min(curPos.X, minX);
                minY = Math.Min(curPos.Y, minY);
                maxX = Math.Max(curPos.X, maxX);
                maxY = Math.Max(curPos.Y, maxY);
            }
            var map = new int[maxX - minX + 1, maxY - minY + 1];
            curPos = new Point(-minX, -minY);
            var n = 1;
            for (int i = 0; i < instructions.Count; i++)
            {
                var nextPos = instructions[i].Move(curPos);
                switch (instructions[i].direction)
                {
                    case dir.up:
                        for (int x = curPos.X - 1; x >= nextPos.X; x--)
                        {
                            map[x, curPos.Y] = n;
                            n++;
                        }
                        break;
                    case dir.down:
                        for (int x = curPos.X + 1; x <= nextPos.X; x++)
                        {
                            map[x, curPos.Y] = n;
                            n++;
                        }
                        break;
                    case dir.left:
                        for (int y = curPos.Y - 1; y >= nextPos.Y; y--)
                        {
                            map[curPos.X, y] = n;
                            n++;
                        }
                        break;
                    case dir.right:
                        for (int y = curPos.Y + 1; y <= nextPos.Y; y++)
                        {
                            map[curPos.X, y] = n;
                            n++;
                        }
                        break;
                }
                curPos = nextPos;
            }
            n--;
            for (int i = 0; i < map.GetLength(0) - 1; i++)
            {
                int crossCounter = 0;
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    int a = map[i, j];
                    if (a > 0)
                    {
                        int b = map[i + 1, j];
                        if (b > 0)
                        {
                            if (a == b + 1 || (a == 1 && b == n))
                            {
                                crossCounter++;
                            }
                            else if (a == b - 1 || (b == 1 && a == n))
                            {
                                crossCounter--;
                            }
                        }
                    }
                    else
                    {
                        if (crossCounter > 0)
                        {
                            map[i, j] = -1;
                        }
                    }
                }
            }
            Print(map);
            total1 = map.AsEnumerable().Count(i => i != 0);
            return total1;
        }

        private void Print(int[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.Write(map[i, j] == 0 ? " " : (map[i, j] == -1 ? "*" : "#"));
                }
                Console.WriteLine();
            }
        }

        private void Print(List<(turn t, int length)> turns, dir firstDir)
        {
            var inst = new List<instruction>();
            var curDir = firstDir;
            for (int i = 0; i < turns.Count; i++)
            {
                inst.Add(new instruction() { direction = curDir, length = turns[i].length });
                curDir = NewDirection(curDir, turns[i].t);
            }
            Algo1(inst);
        }

        private void CheckLoop(List<(turn t, int length)> turns, dir firstDir)
        {
            var inst = new List<instruction>();
            var curDir = firstDir;
            for (int i = 0; i < turns.Count; i++)
            {
                inst.Add(new instruction() { direction = curDir, length = turns[i].length });
                curDir = NewDirection(curDir, turns[i].t);
            }
            Point curPos = new Point(0, 0);
            foreach (var instruction in inst)
            {
                curPos = instruction.Move(curPos);
            }
            if (curPos.X != 0 || curPos.Y != 0)
            {
                Console.WriteLine("Not a loop anymore !");
            }
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
    }
}

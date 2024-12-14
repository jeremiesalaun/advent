using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    internal class Day14
    {
        const bool TEST = false;

        private class Robot
        {
            public Point Position { get; set; }
            public Point Velocity { get; set; }

            public void Move()
            {
                Position = Position.Add(Velocity);
                Position = Position.Modulo(roomSize);
            }
        }

        private List<Robot> robots = new List<Robot>();
        private static Point roomSize;

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 14 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day14.txt";
            roomSize = TEST ? new Point(11, 7) : new Point(101, 103);

            foreach (var line in File.ReadAllLines(path))
            {
                ProcessLine(line);
            }
            for (int i = 0; i < 100; i++)
            {
                robots.ForEach(r=>r.Move());
            }
            int q1 = 0, q2 = 0, q3 = 0, q4 = 0;
            foreach (var r in robots)
            {
                if (r.Position.X < roomSize.X / 2 && r.Position.Y < roomSize.Y / 2)
                {
                    q1++;
                }
                else if (r.Position.X < roomSize.X / 2 && r.Position.Y > roomSize.Y / 2)
                {
                    q2++;
                }
                else if (r.Position.X > roomSize.X / 2 && r.Position.Y < roomSize.Y / 2)
                {
                    q3++;
                }
                else if (r.Position.X > roomSize.X / 2 && r.Position.Y > roomSize.Y / 2)
                {
                    q4++;
                }
            }
            total1 = q1 * q2 * q3 * q4;
            int time = 100;
            while (true)
            {
                robots.ForEach(r => r.Move());
                time++;
                //Wait until a position where at least 10 robots are in a straight line (same X, consecutive Y), and ask the user if this is correct.
                for(int i = 0; i < roomSize.X; i++)
                {
                    if(robots.Where(r=>r.Position.X == i).Count() > 10)
                    {
                        var ys = robots.Where(r => r.Position.X == i).Select(r => r.Position.Y).Order().ToList();
                        int streak = 0;
                        for(int j=1; j < ys.Count; j++)
                        {
                            if (ys[j] - ys[j-1]==1) {
                                streak++;
                                if (streak == 10) break;
                            }
                            else
                            {
                                streak = 0;
                            }
                        }
                        if (streak == 10) {
                            Console.Clear();
                            Console.WriteLine($"Disposition after {time}s");
                            PrintOut();
                            Console.Write("If this is the correct pattern: type Y :");
                            if (Console.ReadLine() == "Y")
                            {
                                goto endloop;
                            }
                            else { 
                                break; //Next move
                            }
                        }
                    }
                }
            }
        endloop:
            total2 = time;

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 14 ***********************************");
            Thread.Sleep(1000);
        }

        private void PrintOut()
        {
            for (int i = 0; i < roomSize.X; i++)
            {
                for(int j = 0; j < roomSize.Y; j++)
                {
                    if (robots.Exists(r=>r.Position.Y==i && r.Position.X == j))
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }

        private Regex reRobot = new Regex(@"p=(?<pX>-?\d+),(?<pY>-?\d+) v=(?<vX>-?\d+),(?<vY>-?\d+)");
        private void ProcessLine(string line)
        {
            var m = reRobot.Match(line);
            if (m.Success)
            {
                var r = new Robot()
                {
                    Position = new Point(int.Parse(m.Groups["pX"].Value), int.Parse(m.Groups["pY"].Value)),
                    Velocity = new Point(int.Parse(m.Groups["vX"].Value), int.Parse(m.Groups["vY"].Value)),
                };
                robots.Add(r);
            }
        }
    }
}


using AdventOfCode2025.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2025
{
    //Replace 9 by day number
    internal class Day9
    {
        const bool TEST = false;

        private enum Orientations
        {
            Horizontal, Vertical
        }
        private class Segment
        {
            public Point Start { get; set; }
            public Point End { get; set; }
            public Orientations Orientation { get; set; }
            public int ConstOrdinate { get; set; }
            public override string ToString()
            {
                return $"{Start}->{End} (going {Orientation}";
            }
        }

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 9  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day9.txt";
            var redpoints = File.ReadAllLines(path)
                                .Select(l => new Point(
                                                int.Parse(l.Substring(0, l.IndexOf(','))),
                                                int.Parse(l.Substring(l.IndexOf(',') + 1))))
                                .ToList();


            for (int i = 0; i < redpoints.Count - 1; i++)
            {
                var p1 = redpoints[i];
                for (int j = i + 1; j < redpoints.Count; j++)
                {
                    var p2 = redpoints[j];
                    var area = (long)(Math.Abs(p1.X - p2.X) + 1) * (long)(Math.Abs(p1.Y - p2.Y) + 1);
                    total1 = Math.Max(total1, area);
                }
            }

            /////////////////////////////////

            //Find the outer bound (min X)
            //Build a list of segments finding the segment  direction and if outside is babord or tribord
            var segments = new List<Segment>();
            for (int i = 0; i < redpoints.Count; i++)
            {
                var p1 = redpoints[i];
                var p2 = redpoints[i<redpoints.Count-1 ? i + 1 : 0];
                Orientations d;
                int c;
                if (p1.X == p2.X)
                {
                    d = Orientations.Vertical;
                    c = p1.X;
                }
                else
                {
                    d = Orientations.Horizontal;
                    c = p1.Y;
                }
                segments.Add(new Segment() { Start = p1, End = p2, Orientation = d, ConstOrdinate=c });
            }            

            //For each couple of red tiles, find if there are some segments within the rectangle
            for (int i = 0; i < redpoints.Count - 1; i++)
            {
                var p1 = redpoints[i];
                for (int j = i + 1; j < redpoints.Count; j++)
                {
                    var p2 = redpoints[j];
                    //Console.WriteLine($"Considering {p1} and {p2}");
                    //Determine the 4 segments that constinute the rectangle
                    var minX = Math.Min(p1.X, p2.X);
                    var maxX = Math.Max(p1.X, p2.X);
                    var minY = Math.Min(p1.Y, p2.Y);
                    var maxY = Math.Max(p1.Y, p2.Y);
                    var r = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                    //Are there any horizontal segments between south and north ?
                    Segment? z=null;
                    z = segments.Where(s => s.Orientation == Orientations.Horizontal
                                            && s.ConstOrdinate > r.Top
                                            && s.ConstOrdinate < r.Bottom
                                            && Math.Max(s.Start.X, s.End.X) >= r.Left
                                            && Math.Min(s.Start.X, s.End.X) <= r.Right
                                        ).FirstOrDefault();
                    if (z != null)
                    {
                        //Console.WriteLine($"Impossible because of {z.Orientation} segment {z.Start}-{z.End}");
                        continue;
                    }

                    //Are there any vertical segments between west and east ?
                    z = segments.Where(s => s.Orientation == Orientations.Vertical
                                        && s.ConstOrdinate > r.Left
                                        && s.ConstOrdinate < r.Right
                                        && Math.Max(s.Start.Y, s.End.Y) >= r.Top
                                        && Math.Min(s.Start.Y, s.End.Y) <= r.Bottom
                                    ).FirstOrDefault();
                    if (z != null)
                    {
                        //Console.WriteLine($"Impossible because of {z.Orientation} segment {z.Start}-{z.End}");
                        continue;
                    }

                    //We found a match with no segment within the rectangle
                    var area = (long)(Math.Abs(p1.X - p2.X) + 1) * (long)(Math.Abs(p1.Y - p2.Y) + 1);
                    total2 = Math.Max(total2, area);
                    if(total2==area)
                    {
                        Console.WriteLine($"Found a new max of {area} between {p1} and {p2}");
                    }
                }
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 9 ***********************************");
            Thread.Sleep(1000);
        }

    }
}


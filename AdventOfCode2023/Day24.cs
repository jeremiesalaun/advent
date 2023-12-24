using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AdventOfCode2023
{
    internal class Day24
    {
        private struct Point3D
        {
            public long X { get; set; }
            public long Y { get; set; }
            public long Z { get; set; }

            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                var p = (Point3D)obj;
                return (X == p.X && Y == p.Y && Z == p.Z);
            }

            public Point3D(long x, long y, long z)
            {
                this.X = x; this.Y = y; this.Z = z;
            }

            public override string ToString()
            {
                return $"[{X:N0},{Y:N0},{Z:N0}]";
            }
        }

        private class PointComparerByZ : IComparer<Point3D>
        {
            public int Compare(Point3D a, Point3D b)
            {
                return (int)(a.Z - b.Z);
            }
        }

        private class HailStone
        {
            public string Name { get; set; }
            public Point3D Position { get; set; }
            public Point3D Velocity { get; set; }

            public (double a, double b) GetAffine()
            {
                double a = (double)Velocity.Y / Velocity.X;
                double b = Position.Y - a * Position.X;
                return (a, b);
            }

            public override string ToString()
            {
                return $"{Name} [velocity {Velocity}]";
            }

            internal double GetY(double xinter)
            {
                var a = GetAffine();
                return a.a * xinter + a.b;
            }

            internal bool IsInFuture((double x, double y) value)
            {
                if(Velocity.X>0 && value.x>Position.X)
                {
                    return true;
                }
                if(Velocity.X<0 && value.x < Position.X)
                {
                    return true;
                }
                if (Velocity.X == 0)
                {
                    if (Velocity.Y > 0 && value.y > Position.Y)
                    {
                        return true;
                    }
                    if (Velocity.Y < 0 && value.y < Position.Y)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private List<HailStone> hailstones = [];
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 24 !! ##################################\r\n");
            //Read and parse file
            var namer = new Namer();
            var inputpath = @"Inputs\day24.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                while (!sr.EndOfStream)
                {
                    hailstones.Add(ParseLine(sr.ReadLine(), namer.NextLetter()));
                }
            }

            long total1 = 0;

            long xMin = 200_000_000_000_000;
            long xMax = 400_000_000_000_000;
            long yMin = 200_000_000_000_000;
            long yMax = 400_000_000_000_000;

            for (int i = 0; i < hailstones.Count - 1; i++)
            {
                for (int j = i + 1; j < hailstones.Count; j++)
                {
                    var intersect = GetIntersection(hailstones[i], hailstones[j]);
                    if (intersect == null)
                    {
                        //Console.WriteLine($"{hailstones[i]} and {hailstones[j]} are parrallel");
                    }
                    else
                    {
                        if (intersect.Value.x >= xMin && intersect.Value.x <= xMax && intersect.Value.y >= yMin && intersect.Value.y <= yMax)
                        {
                            if (!hailstones[i].IsInFuture(intersect.Value))
                            {
                                //Console.WriteLine($"Intersection  at [{intersect.Value.x},{intersect.Value.y}] is in the past for {hailstones[i]} ");
                                continue;
                            }
                            if (!hailstones[j].IsInFuture(intersect.Value))
                            {
                                //Console.WriteLine($"Intersection  at [{intersect.Value.x},{intersect.Value.y}] is in the past for {hailstones[j]} ");
                                continue;
                            }
                            //Console.WriteLine($"{hailstones[i]} and {hailstones[j]} intersects  at [{intersect.Value.x},{intersect.Value.y}] inside of the interval");
                            total1++;
                        }
                        else
                        {
                            //Console.WriteLine($"{hailstones[i]} and {hailstones[j]} intersects at [{intersect.Value.x},{intersect.Value.y}] out of the interval");
                        }
                    }
                }
            }
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;
            hailstones.Sort((h1, h2) => h1.Position.Z == h2.Position.Z ? 0 : (h1.Position.Z - h2.Position.Z > 0 ? 1 : -1));


            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 24 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private (double x, double y)? GetIntersection(HailStone h1, HailStone h2)
        {
            var affine1 = h1.GetAffine();
            var affine2 = h2.GetAffine();
            if (affine1.a == affine2.a)
            {
                //Parallel
                return null;
            }
            var xinter = (double)(affine2.b - affine1.b) / (affine1.a - affine2.a);
            var yinter = h1.GetY(xinter);
            return (xinter, yinter);
        }


        private HailStone ParseLine(string? line, string name)
        {
            var re = new Regex(@"(?<x1>\d+),\s+(?<y1>\d+),\s+(?<z1>\d+)\s+@\s+(?<x2>[-]?\d+),\s+(?<y2>[-]?\d+),\s+(?<z2>[-]?\d+)");
            var match = re.Match(line);
            if (match.Success)
            {
                var h = new HailStone();
                h.Name = name;
                h.Position = new Point3D()
                {
                    X = long.Parse(match.Groups["x1"].Value),
                    Y = long.Parse(match.Groups["y1"].Value),
                    Z = long.Parse(match.Groups["z1"].Value),
                };
                h.Velocity = new Point3D()
                {
                    X = long.Parse(match.Groups["x2"].Value),
                    Y = long.Parse(match.Groups["y2"].Value),
                    Z = long.Parse(match.Groups["z2"].Value),
                };
                return h;
            }
            return null;
        }
    }
}

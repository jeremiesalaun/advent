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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AdventOfCode2023
{
    internal class Day22
    {
        private struct Point3D
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                var p = (Point3D)obj;
                return (X == p.X && Y == p.Y && Z == p.Z);
            }

            public Point3D(int x, int y, int z) { 
                this.X = x; this.Y = y; this.Z = z;
            }
        }

        private class PointComparerByZ : IComparer<Point3D>
        {
            public int Compare(Point3D a, Point3D b)
            {
                return a.Z - b.Z;
            }
        }

        private class BrickComparerByZ : IComparer<Brick>
        {
            public int Compare(Brick a, Brick b)
            {
                var res =  a.Bottom.Z - b.Bottom.Z;
                if(res==0) res = a.Base.X - b.Base.X;
                if(res==0) res = a.Base.Y - b.Base.Y;
                return res;
            }
        }

        private class Brick
        {
            public string Name { get; set; }
            public Point3D Bottom { get; set; }
            public Point3D Top { get; set; }

            private Rectangle? _base;
            public Rectangle Base
            {
                get
                {
                    if(!_base.HasValue)
                    {
                        _base = new Rectangle(Math.Min(Top.X, Bottom.X), Math.Min(Top.Y, Bottom.Y), Math.Abs(Top.X - Bottom.X), Math.Abs(Top.Y - Bottom.Y));
                    }
                    return _base.Value;
                }
            }

            internal void SetLevel(int v)
            {
                var height = this.Top.Z -this.Bottom.Z;
                this.Bottom = new Point3D(this.Bottom.X,this.Bottom.Y,v);
                this.Top = new Point3D(this.Top.X, this.Top.Y, v + height);
            }

            public bool BaseIntersectsWith(Brick b)
            {
                return (this.Base.Left <= b.Base.Right 
                    && this.Base.Right >= b.Base.Left 
                    && this.Base.Top <= b.Base.Bottom 
                    && this.Base.Bottom >= b.Base.Top);
            }
            public override string ToString()
            {
                return $"{Name} [{Base}, level {Bottom.Z}]";
            }
        }

        private List<Brick> bricks = [];
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 22 !! ##################################\r\n");
            //Read and parse file
            var namer = new Namer();
            var inputpath = @"Inputs\day22.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                while (!sr.EndOfStream)
                {
                    var b = ParseLine(sr.ReadLine());
                    b.Name = namer.NextName();
                    bricks.Add(b);
                }
            }

            bricks.Sort(new BrickComparerByZ());

            var maxX = bricks.Max(b => b.Base.Right);
            var maxY = bricks.Max(b => b.Base.Bottom);
            var baseLevels = new int[maxX + 1, maxY + 1];

            //Make all the bricks fall to the lowest level
            foreach (var b in bricks)
            {
                int maxLevel = 0;
                for (int x = b.Base.Left; x <= b.Base.Right; x++)
                {
                    for (int y = b.Base.Top; y<= b.Base.Bottom; y++)
                    {
                        maxLevel = Math.Max(maxLevel, baseLevels[x,y]);
                    }
                }
                //Console.WriteLine($"{b.Name} [{b.Base}] was {b.Bottom.Z} and is now {maxLevel+1}");
                b.SetLevel(maxLevel+1);
                for (int x = b.Base.Left; x <= b.Base.Right; x++)
                {
                    for (int y = b.Base.Top; y <= b.Base.Bottom; y++)
                    {
                        baseLevels[x, y]=b.Top.Z;
                    }
                }
            }
            //For every brick, find out who is supporting them
            var supporters = new Dictionary<Brick, List<Brick>>();
            bricks.Sort(new BrickComparerByZ());
            foreach (var b in bricks)
            {
                if (b.Bottom.Z > 1)
                {
                    var sups = bricks.Where(c => c.Top.Z == b.Bottom.Z-1 && b.BaseIntersectsWith(c)).ToList();
                    supporters.Add(b,sups);
                }
            }
            
            
            
            //We can disintegrate every brick that is not the single support of another.
            long total1 = 0;
            var couldBeDisintegrated = new List<Brick>(bricks);
            foreach(var b in bricks)
            {
                if (supporters.Where(kvp=>kvp.Value.Contains(b) && kvp.Value.Count == 1).Any())
                {
                    couldBeDisintegrated.Remove(b);
                }
            }
            total1 = couldBeDisintegrated.Count();
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;

            //For each brick, find out how many brick would fall if it disintegrates
            var res = new Dictionary<Brick, int>();
            foreach (var disBrick in bricks)
            {
                var alreadyFallen = new List<Brick>();
                var q = new Queue<Brick>();
                q.Enqueue(disBrick);
                while (q.Any())
                {
                    var b = q.Dequeue();
                    alreadyFallen.Add(b);
                    var willFall = supporters.Where(kvp => kvp.Value.Contains(b) && kvp.Value.TrueForAll(x=>alreadyFallen.Contains(x)))
                                            .Select(kvp=>kvp.Key).ToList();
                    willFall.ForEach(x => q.Enqueue(x));
                }
                res.Add(disBrick, alreadyFallen.Count-1);
            }
            total2 = res.Sum(kvp => kvp.Value);

            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 22 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private Brick ParseLine(string? line)
        {
            var re = new Regex(@"(?<x1>\d+),(?<y1>\d+),(?<z1>\d+)~(?<x2>\d+),(?<y2>\d+),(?<z2>\d+)");
            var match = re.Match(line);
            if (match.Success)
            {
                var b = new Brick();
                var p1 = new Point3D()
                {
                    X = int.Parse(match.Groups["x1"].Value),
                    Y = int.Parse(match.Groups["y1"].Value),
                    Z = int.Parse(match.Groups["z1"].Value),
                };
                var p2 = new Point3D()
                {
                    X = int.Parse(match.Groups["x2"].Value),
                    Y = int.Parse(match.Groups["y2"].Value),
                    Z = int.Parse(match.Groups["z2"].Value),
                };
                if (p2.Z < p1.Z)
                {
                    b.Bottom = p2;
                    b.Top = p1;
                }
                else
                {
                    b.Bottom = p1;
                    b.Top = p2;
                }
                return b;
            }
            return null;
        }
    }
}

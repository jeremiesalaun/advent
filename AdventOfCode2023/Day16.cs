using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Drawing;
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
    internal class Day16
    {
        private const int MapLengthX = 110;
        private const int MapLengthY = 110;
        private enum dir
        {
            north, south, west, east
        }

        private class Ray
        {
            public Point position { get; set; }
            public dir direction { get; set; }
        }

        private class Tile
        {
            public char Kind { get; set; }
            public bool Energized { get; set; }

            public HashSet<dir> AlreadyEnteredFrom { get; set; } = [];

            public (dir? output1, dir? output2) Energize(dir input)
            {
                this.Energized = true;
                if (AlreadyEnteredFrom.Contains(input))
                {
                    return (null, null);
                }
                AlreadyEnteredFrom.Add(input);
                switch (Kind)
                {
                    case '-':
                        if (input == dir.west || input == dir.east)
                        {
                            return (input, null);
                        }
                        else
                        {
                            return (dir.east, dir.west);
                        }
                    case '|':
                        if (input == dir.north || input == dir.south)
                        {
                            return (input, null);
                        }
                        else
                        {
                            return (dir.north, dir.south);
                        }
                    case '/':
                        if (input == dir.east) return (dir.north, null);
                        if (input == dir.north) return (dir.east, null);
                        if (input == dir.west) return (dir.south, null);
                        if (input == dir.south) return (dir.west, null);
                        break;
                    case '\\':
                        if (input == dir.east) return (dir.south, null);
                        if (input == dir.north) return (dir.west, null);
                        if (input == dir.west) return (dir.north, null);
                        if (input == dir.south) return (dir.east, null);
                        break;
                }
                return (input, null);
            }
        }

        private Tile[,] map;
        private Dictionary<int, Tile[,]> mapCache = [];

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 16 !! ##################################\r\n");
            //Read and parse file
            map = new Tile[MapLengthX, MapLengthY];

            var inputpath = @"Inputs\day16.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                int i = 0;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().ToCharArray();
                    for (int j = 0; j < line.Length; j++)
                    {
                        map[i, j] = new Tile() { Kind = line[j] };
                    }
                    i++;
                }
            }
            //Print(map);
            long total1 = TestRayInput(new Point(0, 0), dir.east);

            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;
            //Init the collection of rays to test
            var results = new Dictionary<(Point p, dir d), long>();
            for (int i = 0; i < MapLengthX; i++)
            {
                results[(new Point(i, 0), dir.east)] = 0;
                results[(new Point(i, MapLengthY - 1), dir.west)] = 0;
            }
            for (int j = 0; j < MapLengthY; j++)
            {
                results[(new Point(0, j), dir.south)] = 0;
                results[(new Point(MapLengthX-1, j), dir.north)] = 0;
            }

            //Lauch testing in parallel
            Parallel.ForEach(results,
                new ParallelOptions() { MaxDegreeOfParallelism = 6 },
                kvp => results[kvp.Key] = TestRayInput(kvp.Key.p, kvp.Key.d));

            total2 = results.Max(kvp => kvp.Value);
            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 16 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private Tile[,] CloneMap()
        {
            var res = new Tile[MapLengthX, MapLengthY];
            res.ForEachIndex(p => res[p.X, p.Y]= new Tile() { Kind = map[p.X,p.Y].Kind });
            return res;
        }

        private long TestRayInput(Point initial, dir direction)
        {
            //The fact that we need a copy of the map for each calculation cancels the gain from parallel computation.
            //So I implemented a small cache so that each thread have its own map that it can reuse each time
            Tile[,] myMap = GetMapFromCache();

            var rays = new List<Ray>() { new Ray() { position = initial, direction = direction } };
            while (rays.Count > 0)
            {
                for (int i = rays.Count - 1; i >= 0; i--)
                {
                    var ray = rays[i];
                    var newDirs = myMap[ray.position.X, ray.position.Y].Energize(ray.direction);
                    if (newDirs.output1.HasValue)
                    {
                        var p = Move(ray.position, newDirs.output1.Value);
                        if (p != null)
                        {
                            ray.position = p.Value;
                            ray.direction = newDirs.output1.Value;
                        }
                        else
                        {
                            rays.RemoveAt(i);
                        }
                        if (newDirs.output2.HasValue)
                        {
                            var p2 = Move(ray.position, newDirs.output2.Value);
                            if (p2 != null)
                            {
                                rays.Add(new Ray() { position = p2.Value, direction = newDirs.output2.Value });
                            }
                        }
                    }
                    else
                    {
                        rays.RemoveAt(i);
                    }
                }
            }
            return Evaluate(myMap);
        }

        private Tile[,] GetMapFromCache()
        {
            Tile[,] myMap;
            if (mapCache.TryGetValue(Environment.CurrentManagedThreadId, out Tile[,]? value))
            {
                myMap = value;
                foreach (Tile t in myMap)
                {
                    t.Energized = false;
                    t.AlreadyEnteredFrom.Clear();
                }
            }
            else
            {
                myMap = CloneMap();
                mapCache.Add(Environment.CurrentManagedThreadId, myMap);
            }

            return myMap;
        }

        private long Evaluate(Tile[,] map)
        {
            return map.AsEnumerable().Sum(t => t.Energized ? 1 : 0);
        }

        private void Print(Tile[,] map)
        {
            Console.Clear();
            map.ForEachIndex(p =>
            {
                if (p.X > 0 && p.Y == 0) Console.WriteLine();
                if (map[p.X, p.Y].Energized) Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(map[p.X, p.Y].Kind);
                Console.ResetColor();
            });
        }

        private Point? Move(Point position, dir direction)
        {
            switch (direction)
            {
                case dir.north: position.X--; break;
                case dir.south: position.X++; break;
                case dir.east: position.Y++; break;
                case dir.west: position.Y--; break;
            }
            //Check for out of bounds
            if (position.X < 0 || position.X > MapLengthX - 1) return null;
            if (position.Y < 0 || position.Y > MapLengthY - 1) return null;
            return position;
        }
    }
}

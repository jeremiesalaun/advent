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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AdventOfCode2023
{
    internal class Day23
    {
        private const int MapLengthX = 141;
        private const int MapLengthY = 141;
        private enum dir
        {
            none, north, south, west, east
        }

        private char[,] map;

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 23 !! ##################################\r\n");
            //Read and parse file
            map = new char[MapLengthX, MapLengthY];

            var inputpath = @"Inputs\day23.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                int i = 0;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().ToCharArray();
                    for (int j = 0; j < line.Length; j++)
                    {
                        map[i, j] = line[j];
                    }
                    i++;
                }
            }
            //Print(map);
            long total1 = 0;
            var path = new List<Point>() { new Point(0, 1) };
            var results = new List<List<Point>>();
            var q = new Queue<List<Point>>();
            q.Enqueue(path);
            while (q.Count > 0)
            {
                var p = q.Dequeue();
                if (p.Last().X == MapLengthX - 1)
                {
                    //Arrived
                    results.Add(p);
                    continue;
                }
                MoveAndEnqueue(q, p, dir.north);
                MoveAndEnqueue(q, p, dir.south);
                MoveAndEnqueue(q, p, dir.east);
                MoveAndEnqueue(q, p, dir.west);
            }
            total1 = results.Max(p => p.Count - 1);
            Console.WriteLine($"Final result for 1st star is : {total1}");

            var progress = new AllIWantIsProgress();
            long total2 = 0;

            //Find all the nodes in the maze and the distance between them
            //Find the longest path that goes from start to end, it means passing through as much nodes as possible
            //while still having the longest path.
            var namer = new Namer();
            var nodes = new List<Node>();
            nodes.Add(new Node() { Name = "START", Position = new Point(0, 1) });
            nodes.Add(new Node() { Name = "FINISH", Position = new Point(MapLengthX - 1, MapLengthY - 2) });

            var nq = new Queue<Node>();
            nq.Enqueue(nodes.First());
            while (nq.Any())
            {
                var curNode = nq.Dequeue();
                foreach (var curDir in PossibleMoves(curNode.Position, dir.none))
                {
                    if (curNode.Neighbors.Any(n => n.dir == curDir))
                    {
                        continue;
                    }
                    var curPos = curNode.Position;
                    dir lastDir = curDir;
                    int steps = 1;
                    curPos = Move(curPos, curDir).Value;
                    while (true)
                    {
                        if (steps > 0 && nodes.Any(n => n.Position == curPos))
                        {
                            var newNode = nodes.FirstOrDefault(n => n.Position == curPos);
                            curNode.Neighbors.Add((newNode, curDir, steps));
                            newNode.Neighbors.Add((curNode, ReverseDir(lastDir), steps));
                            break;
                        }
                        else
                        {
                            List<dir> dirs = PossibleMoves(curPos, lastDir);
                            if (dirs.Count == 1)
                            {
                                var p = Move(curPos, dirs[0]);
                                if (p != null)
                                {
                                    steps++;
                                    curPos = p.Value;
                                    lastDir = dirs[0];
                                }
                            }
                            else if (dirs.Count > 1)
                            {
                                //This is a new node.
                                var n = new Node() { Name = namer.NextLetter(), Position = curPos };
                                nodes.Add(n);
                                curNode.Neighbors.Add((n, curDir, steps));
                                n.Neighbors.Add((curNode, ReverseDir(lastDir), steps));
                                nq.Enqueue(n);
                                break;
                            }
                        }
                    }
                }
            }

            var start = nodes.First();
            total2 = StepsToFinish(start, 0, new List<Node>());


            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 23 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private int StepsToFinish(Node startNode, int currentSteps, List<Node> path)
        {
            var res = 0;
            foreach (var neighbor in startNode.Neighbors.OrderByDescending(n => n.dist))
            {
                if (!path.Contains(neighbor.n))
                {
                    int d;
                    if (neighbor.n.Name == "FINISH")
                    {
                        d = currentSteps + neighbor.dist;
                        //foreach(var n in path)
                        //{
                        //    Console.Write($"{n.Name}, ");
                        //}
                        //Console.WriteLine();
                    }
                    else
                    {
                        var pathClone = new List<Node>(path) { neighbor.n };
                        d = StepsToFinish(neighbor.n, currentSteps + neighbor.dist, pathClone);
                    }
                    res = Math.Max(res, d);
                }
            }
            return res;
        }

        private List<dir> PossibleMoves(Point curPos, dir d)
        {
            var rd = ReverseDir(d);
            var res = new List<dir>();
            for (int i = 1; i < 5; i++)
            {
                dir dir = (dir)i;
                if (rd != dir)
                {
                    var p = Move(curPos, dir);
                    if (p != null && map[p.Value.X, p.Value.Y] != '#')
                    {
                        res.Add(dir);
                    }
                }
            }
            return res;
        }

        private dir ReverseDir(dir d)
        {
            switch (d)
            {
                case dir.north: return dir.south;
                case dir.south: return dir.north;
                case dir.west: return dir.east;
                default: return dir.west;
            }
        }

        private class Node
        {
            public Point Position { get; set; }
            public List<(Node n, dir dir, int dist)> Neighbors { get; set; } = [];
            public string Name { get; set; }

            public override string ToString()
            {
                return $"{Name} [{Position.X},{Position.Y}]";
            }
        }

        private void MoveAndEnqueue(Queue<List<Point>> q, List<Point> currentPath, dir direction, bool allowSlopes = false)
        {
            var curPos = currentPath.Last();
            var p = Move(curPos, direction);
            if (p != null && !currentPath.Contains(p.Value))
            {
                var clone = ClonePath(currentPath);
                switch (map[p.Value.X, p.Value.Y])
                {
                    case '#': return;
                    case '^': if (!allowSlopes && direction == dir.south) { return; } break;
                    case 'v': if (!allowSlopes && direction == dir.north) { return; } break;
                    case '<': if (!allowSlopes && direction == dir.east) { return; } break;
                    case '>': if (!allowSlopes && direction == dir.west) { return; } break;
                }
                clone.Add(p.Value);
                q.Enqueue(clone);
            }
        }

        private void MoveAndEnqueue(PriorityQueue<List<Point>, int> pq, List<Point> currentPath, bool allowSlopes = false)
        {
            var lp = new List<Point>();
            //Move to the next point of multiple choice
            while (true)
            {
                var p = MoveAndCheck(currentPath, dir.north, allowSlopes);
                if (p != null) lp.Add(p.Value);
                p = MoveAndCheck(currentPath, dir.south, allowSlopes);
                if (p != null) lp.Add(p.Value);
                p = MoveAndCheck(currentPath, dir.west, allowSlopes);
                if (p != null) lp.Add(p.Value);
                p = MoveAndCheck(currentPath, dir.east, allowSlopes);
                if (p != null) lp.Add(p.Value);
                if (lp.Count == 0)
                {
                    //No more choice : this is a dead end.
                    return;
                }
                else if (lp.Count == 1)
                {
                    //Only one choice
                    currentPath.Add(lp[0]);
                    if (lp[0].X == MapLengthX - 1)
                    {
                        //If we arrived.
                        pq.Enqueue(currentPath, int.MinValue);
                        return;
                    }
                    else
                    {
                        //Continue to walk
                        lp.Clear();
                    }
                }
                else
                {
                    //Multiple choice, exit the loop
                    break;
                }
            }
            //Create a new path for each of the choices
            foreach (var p in lp)
            {
                var clone = ClonePath(currentPath);
                clone.Add(p);
                pq.Enqueue(clone, -currentPath.Count);
            }
        }

        private Point? MoveAndCheck(List<Point> currentPath, dir direction, bool allowSlopes = false)
        {
            var curPos = currentPath.Last();
            var p = Move(curPos, direction);
            if (p != null && !currentPath.Contains(p.Value))
            {
                switch (map[p.Value.X, p.Value.Y])
                {
                    case '#': return null;
                    case '^': if (!allowSlopes && direction == dir.south) { return null; } break;
                    case 'v': if (!allowSlopes && direction == dir.north) { return null; } break;
                    case '<': if (!allowSlopes && direction == dir.east) { return null; } break;
                    case '>': if (!allowSlopes && direction == dir.west) { return null; } break;
                }
                return p;
            }
            return null;
        }

        private List<Point> ClonePath(List<Point> currentPath)
        {
            return new List<Point>(currentPath);
        }

        private char[,] CloneMap()
        {
            var res = new char[MapLengthX, MapLengthY];
            res.ForEachIndex(p => res[p.X, p.Y] = map[p.X, p.Y]);
            return res;
        }

        private long Evaluate(char[,] map)
        {
            return map.AsEnumerable().Sum(c => c == 'O' ? 1 : 0);
        }

        private void Print(char[,] map)
        {
            Console.Clear();
            map.ForEachIndex(p =>
            {
                if (p.X > 0 && p.Y == 0) Console.WriteLine();
                if (map[p.X, p.Y] == 'O') Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(map[p.X, p.Y]);
                Console.ResetColor();
            });
        }

        private Point? Move(Point position, dir direction, bool checkBounds = true)
        {
            switch (direction)
            {
                case dir.north: position.X--; break;
                case dir.south: position.X++; break;
                case dir.west: position.Y--; break;
                case dir.east: position.Y++; break;
            }
            //Check for out of bounds
            if (checkBounds)
            {
                if (position.X < 0 || position.X > MapLengthX - 1) return null;
                if (position.Y < 0 || position.Y > MapLengthY - 1) return null;
            }
            return position;
        }
    }
}

using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    internal class Day16
    {
        private class Node
        {
            public int Id { get; set; }
            public Point Position { get; set; }
            public List<(Node n, dirs d, int cost)> Neighbors { get; set; } = new List<(Node n, dirs d, int cost)>();
            public bool IsExplored { get; set; }
            public override string ToString()
            {
                return $"{Id}[{Position.X},{Position.Y}]";
            }
        }

        const bool TEST = false;
        private char[,] map;
        private List<Node> nodes;
        private IntSequence namer = new IntSequence();
        private List<(HashSet<Node> path, int cost)> possiblePaths;
        private Dictionary<int, int> bestCostByNode;

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 16 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day16.txt";
            map = MapHelper.LoadCharMap(path);
            Point start = map.AsPointEnumerable().Where(p => map.Get(p) == 'S').First();
            Point end = map.AsPointEnumerable().Where(p => map.Get(p) == 'E').First();


            //Build the network of nodes in the map.
            var ns = new Node() { Id = 0, Position = start };
            var ne = new Node() { Id = -1, Position = end };
            nodes = new List<Node>() { ns, ne };
            ExploreNode(ns);

            //We now have a network of nodes
            //map.Print();

            bestCostByNode = new Dictionary<int, int>();
            possiblePaths = new List<(HashSet<Node> path, int cost)>();
            var currentPath = new HashSet<Node>() { ns };
            var currentCost = 0;
            FindPath(ns, dirs.east, ne, currentPath, currentCost);


            map = MapHelper.LoadCharMap(path); //Reset the map for printout
            var bestCost = possiblePaths.Min(x => x.cost);
            var bestPaths = possiblePaths.Where(x => x.cost == bestCost).ToList();
            var bestTiles = new HashSet<Point>() { start, end };
            foreach (var p in bestPaths)
            {
                var pathnodes = p.path.ToList();
                var curNode = pathnodes[0];
                var curPos = curNode.Position;
                for (int i = 1; i < pathnodes.Count; i++)
                {
                    var nextNode = pathnodes[i];
                    var dir = curNode.Neighbors.Where(n => n.n.Id == nextNode.Id).Select(n => n.d).First();
                    while (curPos != nextNode.Position)
                    {
                        bestTiles.Add(curPos);
                        map.Set(curPos, 'O');
                        curPos = curPos.Move(dir);
                    }
                    curNode = nextNode;
                }
            }
            map.Print(c => c == 'O', ConsoleColor.Red);
            total2 = bestTiles.Count;
            if (bestPaths.Any())
            {
                Console.WriteLine($"Found {bestPaths.Count} best paths");
                Console.WriteLine($"One of the best path is {string.Join(",", bestPaths[0].path.Select(n => n.Id))} for a cost of {bestCost}");
                total1 = bestCost;
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 16 ***********************************");
            Thread.Sleep(1000);
        }

        private void FindPath(Node currentNode, dirs currentDir, Node targetNode, HashSet<Node> currentPath, int currentCost)
        {
            //Using the OrderBy, we start by evaluating the most costly paths (by turning whenever we can),
            //so as not to miss any result.
            foreach (var neighbor in currentNode.Neighbors.OrderBy(n=>n.d==currentDir?1:0))
            {
                if (neighbor.n.Id == targetNode.Id)
                {
                    //Reached target
                    currentPath.Add(targetNode);
                    if (currentDir != neighbor.d) currentCost += 1000;
                    currentCost += neighbor.cost;
                    if (!checkBestCost(targetNode.Id, currentCost))
                    {
                        return; //Found a path, but the cost was not optimal
                    }
                    possiblePaths.Add((currentPath, currentCost));
                    //Console.WriteLine($"Found a new possible path for a cost of {currentCost} in {currentPath.Count} steps");
                }
                else if (!currentPath.Contains(neighbor.n))
                {
                    var c = currentCost;
                    if (currentDir != neighbor.d) c += 1000;
                    c += neighbor.cost;
                    if (!checkBestCost(neighbor.n.Id, c))
                    {
                        //Abandon path as it's too costly
                        Console.Write($"\r{currentPath.Count}"); //=> The calculation finishes when this count goes down to 0.
                        continue;
                    }
                    //Continue exploring path
                    var path = new HashSet<Node>(currentPath) { neighbor.n };
                    FindPath(neighbor.n, neighbor.d, targetNode, path, c);
                }
            }
        }

        private bool checkBestCost(int id, int c)
        {
            if (bestCostByNode.ContainsKey(id))
            {
                if (bestCostByNode[id] < c)
                {
                    return false;
                }
                else
                {
                    bestCostByNode[id] = c;
                    return true;
                }
            }
            else
            {
                bestCostByNode.Add(id, c);
                return true;
            }
        }

        private void ExploreNode(Node n)
        {
            FindNextNode(n, dirs.east);
            FindNextNode(n, dirs.south);
            FindNextNode(n, dirs.west);
            FindNextNode(n, dirs.north);
            n.IsExplored = true;
        }

        private void FindNextNode(Node n, dirs d)
        {
            if (n.Neighbors.Exists(x => x.d == d)) return; //This direction has already been explored
            var p = n.Position;
            var distance = 0;
            while (true)
            {
                if (IsNode(p, d, n))
                {
                    if (map.Get(p) == '.')
                    {
                        var node = new Node() { Id = namer.Next(), Position = p };
                        nodes.Add(node);
                        map.Set(p, 'X');
                        n.Neighbors.Add((node, d, distance));
                        ExploreNode(node);
                    }
                    else
                    {
                        var node = nodes.First(n => n.Position == p);
                        n.Neighbors.Add((node, d, distance));
                        if (!node.Neighbors.Exists(x => x.d == NavigationHelper.Opposite(d)))
                        {
                            node.Neighbors.Add((n, NavigationHelper.Opposite(d), distance));
                        }
                        if (!node.IsExplored) ExploreNode(node);
                    }
                    break;
                }
                else
                {
                    p = p.Move(d);
                    if (map.Get(p) == '#') break;
                    distance++;
                }
            }
        }

        private bool IsNode(Point p, dirs d, Node origin)
        {
            //The current point is a node if :
            // - We encounter a wall
            // - It's already marked as a node (with a letter)
            // - We can either turn left or turn right.
            if (p == origin.Position) return false;
            if (map.Get(p.Move(d)) == '#')
            {
                return true;
            }
            else if (map.Get(p) != '.')
            {
                return true;
            }
            else if (map.Get(p.Move(NavigationHelper.TurnRight(d))) != '#' || map.Get(p.Move(NavigationHelper.TurnLeft(d))) != '#')
            {
                return true;
            }
            return false;
        }
    }
}


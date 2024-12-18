using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    internal class Day18
    {
        private class Node
        {
            public int Id { get; set; }
            public Point Position { get; set; }
            public List<Node> Neighbors { get; set; } = new List<Node>();

            public override string ToString()
            {
                return $"{Position}";
            }
        }
        
        private char[,] map;
        private Node[,] nodeMap;
        private List<Point> input;
        private IntSequence namer = new IntSequence();
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 18 !! ##################################\r\n");
            long total1 = 0;
            string total2="";
            //Read file
            map = TEST ? new char[7, 7] : new char[71, 71];
            map.ForEachIndex(p => map.Set(p, '.'));
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day18.txt";
            input = File.ReadAllLines(path).Select(line => line.Split(",")).Select(s => new Point(int.Parse(s[1]), int.Parse(s[0]))).ToList();

            int steps = TEST ? 12 : 1024;
            for (int i = 0; i < steps; i++)
            {
                map.Set(input[i], '#');
            }

            //Create the nodes network
            nodeMap = TEST ? new Node[7, 7] : new Node[71, 71];
            map.ForEachIndex(p => ProcessNode(p));
            map.Print();

            total1 = RunWaterFill();

            for (int i = 1024; i < input.Count; i++)
            {
                var p = input[i];
                map.Set(input[i], '#');
                var node = nodeMap.Get(p);
                nodeMap.ForEach(n =>
                {
                    if (n != null)
                    {
                        n.Neighbors.Remove(node);
                    }
                });
                nodeMap.Set(p, null);
                var n = RunWaterFill();
                if (n == 0)
                {
                    total2 = $"{p.Y},{p.X}";
                    break;
                }
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 18 ***********************************");
            Thread.Sleep(1000);
        }

        private long RunWaterFill()
        {
            var waterMap = (char[,])map.Clone();
            var distMap = TEST ? new int[7, 7] : new int[71, 71];
            var queue = new Queue<(Point p, int step, char c)>();
            queue.Enqueue((new Point(0, 0), 0, 'o'));
            queue.Enqueue((new Point(70, 70), 0, 'x'));
            while (queue.Count > 0)
            {
                var task = queue.Dequeue();
                if (waterMap.Get(task.p) == task.c) continue;
                if (waterMap.Get(task.p) != '.')
                {
                    //We have contact !
                    //waterMap.Print(c => c == 'o' || c == 'x', ConsoleColor.Blue);
                    //Console.WriteLine($"Made contact after {task.step}");
                    queue.Clear();
                    return distMap.Get(task.p) + task.step;
                }
                else
                {
                    waterMap.Set(task.p, task.c);
                    distMap.Set(task.p, task.step);
                    var n = nodeMap.Get(task.p);
                    if (n != null)
                    {
                        n.Neighbors.ForEach(n => queue.Enqueue((n.Position, task.step + 1, task.c)));
                    }
                }
            }
            waterMap.Print(c => c == 'o' || c == 'x', ConsoleColor.Red);
            return 0;
        }

        private void ProcessNode(Point p)
        {
            if (map.IsOutOfBound(p)) return;
            if (map.Get(p) == '#') return;
            var curNode = nodeMap.Get(p);
            if (curNode == null)
            {
                curNode = new Node()
                {
                    Id = namer.Next(),
                    Position = p,
                };
                nodeMap.Set(p, curNode);
            }
            FindNeighbor(curNode, dirs.south);
            FindNeighbor(curNode, dirs.east);
            FindNeighbor(curNode, dirs.west);
            FindNeighbor(curNode, dirs.north);
        }

        private void FindNeighbor(Node node, dirs d)
        {
            var p = node.Position.Move(d);
            if (map.IsOutOfBound(p)) return;
            if (nodeMap.Get(p) != null)
            {
                var neigh = nodeMap.Get(p);
                if (!node.Neighbors.Contains(neigh)) node.Neighbors.Add(neigh);
                if (!neigh.Neighbors.Contains(node)) neigh.Neighbors.Add(node);
            }
        }
    }
}


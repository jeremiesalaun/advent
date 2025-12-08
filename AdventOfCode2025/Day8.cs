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
    //Replace 8 by day number
    internal class Day8
    {
        private class Point3D
        {
            public int X, Y, Z;
            public string Name;
            public override string ToString()
            {
                return Name;
            }
        }

        private double distance(Point3D p1, Point3D p2) {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2));            
        }


        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 8  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var namer = new Namer();
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day8.txt";
            var points = File.ReadAllLines(path).Select(s => s.Split(',')).Select(a => new Point3D { X=int.Parse(a[0]), Y=int.Parse(a[1]), Z=int.Parse(a[2]),Name=namer.NextName() }).ToList();

            var distances = new List<(Point3D p1, Point3D p2, double distance)>();
            for(int i=0; i<points.Count-1; i++)
            {
                for (int j = i+1; j < points.Count; j++)
                {
                    distances.Add((points[i], points[j], distance(points[i], points[j])));
                }
            }
            distances.Sort((x1, x2) => (int)(x1.distance*1000 - x2.distance * 1000));

            var circuits = new List<HashSet<Point3D>>();
            
            //Compute the circuits for the N smallest distances
            distances.Take(TEST?10:1000).ToList()
                    .ForEach(conn=> addBoxToCircuits(circuits, conn));
            
            //Multiply the size of the 3 largest circuits
            total1 = circuits.OrderByDescending(l => l.Count).Take(3).Select(l => l.Count).Aggregate((l1, l2) => l1 * l2);


            /////
            circuits.Clear();
            foreach(var conn in distances)
            {
                addBoxToCircuits(circuits, conn);
                //If only 1 remaining circuit with all boxes in it
                if(circuits.Count==1 && circuits[0].Count == points.Count)
                {
                    total2 = conn.p1.X * conn.p2.X;
                    break;
                }
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 8 ***********************************");
            Thread.Sleep(1000);
        }

        private static void addBoxToCircuits(List<HashSet<Point3D>> circuits, (Point3D p1, Point3D p2, double distance) conn)
        {
            var existingCircuits = circuits.Where(h => h.Contains(conn.p1) || h.Contains(conn.p2)).ToList();
            if (existingCircuits.Count == 0)
            {
                //Need to create a new circuit
                circuits.Add(new HashSet<Point3D>() { conn.p1,conn.p2 });
            }
            else if (existingCircuits.Count == 2)
            {
                //Merging 2 circuits, by definition the 2 points will already be into the resulting circuit
                var c = existingCircuits[0];
                existingCircuits[1].ToList().ForEach(x => c.Add(x));
                circuits.Remove(existingCircuits[1]);
            }
            else
            {
                //Only one existing circuit, adding the 2 points because we don't know which one is the good one.
                existingCircuits[0].Add(conn.p1);
                existingCircuits[0].Add(conn.p2);
            }
        }
    }
}


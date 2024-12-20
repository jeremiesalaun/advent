using AdventOfCode2024.Helpers;
using System.Data;
using System.Drawing;

namespace AdventOfCode2024
{
    internal class Day12
    {
        const bool TEST = false;
        private char[,] map;
        private int[,] regionsMap;
        private Dictionary<int,(char crop,List<Point> points)> regions;

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 12 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day12.txt";
            map = MapHelper.LoadCharMap(path);
            regionsMap = new int[map.GetLength(0), map.GetLength(1)];
            regions = new Dictionary<int, (char crop, List<Point> points)>();
            var regionId = 1;
            map.ForEachIndex(p => {
                if (regionsMap.Get(p) != 0) return; //Only process slots that have not already been counted in another region
                MapRegion(p,regionId++);
            });
            total1 = regions.Select(kvp => kvp.Value).Sum(v => v.points.Count * GetPerimeter(v.points));
            total2 = regions.Select(kvp => kvp.Value).Sum(v => v.points.Count * GetSides(v.points));


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 12 ***********************************");
            Thread.Sleep(1000);
        }

        private int GetPerimeter(List<Point> points)
        {
            var res = 0;
            foreach (var p in points)
            {
                //Add one to the perimeter for every adjacent of the current slot that is not part of the same region.
                if (!points.Contains(p.Move(dirs.north))) res++;
                if (!points.Contains(p.Move(dirs.east))) res++;
                if (!points.Contains(p.Move(dirs.south))) res++;
                if (!points.Contains(p.Move(dirs.west))) res++;
            }
            return res;
        }

        private int GetSides(List<Point> points)
        {
            var res = 0;
            //A side is determined by an ordinate and a direction (for example X=1 looking north, or Y=2 looking west).
            //For each of this sides, find out all the slots of the region, markin their other coordinate (for example X=1 looking north, Y = [0,1,3,4,8])
            //Then count one side for each continuous serie of coordinates, in our example : [0,1],[3,4] and [8] make 3 sides
            var d = new Dictionary<(int,dirs),List<int>>();
            foreach (var p in points)
            {
                if (!points.Contains(p.Move(dirs.north))){
                    if(!d.ContainsKey((p.X,dirs.north))) d.Add((p.X,dirs.north),new List<int>());
                    d[(p.X, dirs.north)].Add(p.Y);
                }
                if (!points.Contains(p.Move(dirs.south)))
                {
                    if (!d.ContainsKey((p.X, dirs.south))) d.Add((p.X, dirs.south), new List<int>());
                    d[(p.X, dirs.south)].Add(p.Y);
                }
                if (!points.Contains(p.Move(dirs.east)))
                {
                    if (!d.ContainsKey((p.Y, dirs.east))) d.Add((p.Y, dirs.east), new List<int>());
                    d[(p.Y, dirs.east)].Add(p.X);
                }
                if (!points.Contains(p.Move(dirs.west)))
                {
                    if (!d.ContainsKey((p.Y, dirs.west))) d.Add((p.Y, dirs.west), new List<int>());
                    d[(p.Y, dirs.west)].Add(p.X);
                }
            }
            foreach (var k in d.Keys)
            {
                var ordered = d[k].Order().ToList();
                res++;
                for (int i = 1; i < ordered.Count; i++)
                {
                    if ((ordered[i] - ordered[i - 1] != 1)) res++;
                }
            }
            return res;
        }

        private void MapRegion(Point start, int regionId)
        {
            var crop = map.Get(start);
            var points = new List<Point>();
            points.AddRange(PointAndAdjacents(start, crop,regionId));
            regions.Add(regionId, (crop,points));
        }

        private List<Point> PointAndAdjacents(Point start, char crop,int regionId)
        {
            var res = new List<Point>() { start };
            regionsMap.Set(start, regionId); //Mark points that have already been counted
            res.AddRange(CheckAdjacent(start, crop, regionId, dirs.north));
            res.AddRange(CheckAdjacent(start, crop, regionId, dirs.east));
            res.AddRange(CheckAdjacent(start, crop, regionId, dirs.south));
            res.AddRange(CheckAdjacent(start, crop, regionId, dirs.west));
            return res;
        }

        private List<Point> CheckAdjacent(Point start, char crop, int regionId,dirs d)
        {
            var res = new List<Point>();
            var n = start.Move(d);
            //If adjacent spot is inbound and not already counted and has the right crop
            //we can recursively add it and its adjacents
            if (!map.IsOutOfBound(n) && regionsMap.Get(n) == 0 && map.Get(n) == crop)
            {
                res.AddRange(PointAndAdjacents(n, crop, regionId));
            }
            return res;
        }
    }
}


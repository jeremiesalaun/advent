using AdventOfCode2024.Helpers;
using System.Data;
using System.Drawing;

namespace AdventOfCode2024
{
    internal class Day8
    {
        const bool TEST = false;
        private char[,] map;
        private Dictionary<char, List<Point>> locByFreq;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 8 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day8.txt";
            map = MapHelper.LoadCharMap(path);
            locByFreq = map.AsPointEnumerable()
                            .Where(p => map.Get(p) != '.')
                            .Select(p => new { freq = map.Get(p), point = p })
                            .GroupBy(x => x.freq)
                            .ToDictionary(g => g.Key, g => g.Select(x=>x.point).ToList());


            var antinodes = new HashSet<Point>();
            var antinodesFirst = new HashSet<Point>();

            var cnt = locByFreq.Select(kvp => new { f = kvp.Key, p1s = kvp.Value, p2s = kvp.Value })
                            .SelectMany(x => x.p1s, (parent, child) => new { parent.f, p1 = child, parent.p2s })
                            .SelectMany(x => x.p2s, (parent, child) => new { parent.f, parent.p1, p2 = child })
                            .Where(x => x.p1 != x.p2)
                            .Count(x =>
                            {
                                var cnt = 0;
                                var a = x.p1;
                                var dist = x.p1.Substract(x.p2);
                                while (!map.IsOutOfBound(a))
                                {
                                    antinodes.Add(a);
                                    if (cnt == 1) antinodesFirst.Add(a); //For 1st star, only count the first antinode that is not the antenna itself
                                    a = a.Add(dist);
                                    cnt++;
                                }
                                return true;
                            }
                            );
            Console.WriteLine($"Tested {cnt} pairs");
            total1 = antinodesFirst.Count;
            total2 = antinodes.Count;
            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 8 ***********************************");
            Thread.Sleep(1000);
        }      
    }
}
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
    //Replace 11 by day number
    internal class Day11
    {
        const bool TEST = false;
        Dictionary<string, List<string>> devices;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 11  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day11.txt";
            var re = new Regex(@"(?<device>[a-z]+):( (?<output>[a-z]+))+");
            devices = File.ReadAllLines(path).Select(l =>
            {
                var m = re.Match(l);
                if (!m.Success) return null;
                var d = m.Groups["device"].Value;
                var o = new List<string>();
                foreach (Capture c in m.Groups["output"].Captures)
                {
                    o.Add(c.Value);
                }
                return new { device = d, outputs = o };
            }).Where(x => x != null)
            .ToDictionary(x => x.device, x => x.outputs);

            _cache = new Dictionary<(string from, string to, string checkCleared), long>();
            total1 = CountPath("you", "out", new HashSet<string>() { "you" });
            _cache.Clear();
            total2 = CountPath("svr", "out", new HashSet<string>() { "svr" }, new List<string> { "dac","fft"});

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 11 ***********************************");
            Thread.Sleep(1000);
        }

        private Dictionary<(string from, string to,string checkCleared), long> _cache;
        private long CountPath(string from, string to, HashSet<string> alreadyVisited,List<string> check=null)
        {
            var checkCleared = check==null?"": string.Join(',', check.Where(s => alreadyVisited.Contains(s)));
            if (_cache.ContainsKey((from, to, checkCleared))) {
                return _cache[(from, to,checkCleared)];
            }
            if (!devices.ContainsKey(from)) return 0;
            long res = 0;
            foreach (var o in devices[from])
            {
                if(alreadyVisited.Contains(o)) continue;
                if (o == to)
                {
                    if (check==null || check.All(d => alreadyVisited.Contains(d)))
                    {
                        res++;
                    }
                }
                else
                {
                    var av = new HashSet<string>(alreadyVisited) { o };
                    res += CountPath(o, to,av,check);
                }
            }
            _cache[(from, to, checkCleared)] = res;
            return res;
        }
    }
}


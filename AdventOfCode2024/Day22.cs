using System.Data;

namespace AdventOfCode2024
{
    internal class Day22
    {
        const bool TEST = false;
        private List<long> numbers;

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 22 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day22.txt";
            numbers = File.ReadAllLines(path).Select(l => long.Parse(l)).ToList();
            var buyers = new List<Dictionary<(int a, int b, int c, int d), int>>();
            foreach (var n in numbers)
            {
                long x = n;
                (int? a, int? b, int? c, int? d) = (null, null, null, null);
                var values = new Dictionary<(int a, int b, int c, int d),int>();
                //Console.WriteLine($"Generating for number {x}");
                for (int i = 0; i < 2000; i++)
                {
                    var y = GenerateSecret(x);
                    var p = (int)( y % 10);
                    var var = (int)(p - x % 10);
                    a = b; b = c; c = d; d = var;
                    if (a.HasValue && b.HasValue && c.HasValue && d.HasValue)
                    {
                        if(!values.ContainsKey((a.Value, b.Value, c.Value, d.Value)))
                        {
                            values[(a.Value, b.Value, c.Value, d.Value)]=p;
                        }
                    }
                    x = y;
                }
                buyers.Add(values);
                //Console.WriteLine(x);
                total1 += x;
            }

            Console.WriteLine("Calculating best sequence...");
            var aggregated = new Dictionary<(int a, int b, int c, int d), int>();
            foreach(var b in buyers)
            {
                foreach(var k in b.Keys)
                {
                    if (aggregated.ContainsKey(k)) continue;
                    var p = 0;
                    foreach(var b2 in buyers)
                    {
                        if (b2.ContainsKey(k)) { p += b2[k]; }
                    }
                    aggregated.Add(k, p);
                }
            }
            var best = aggregated.OrderByDescending(kvp => kvp.Value).First();
            Console.WriteLine($"{best.Key} => {best.Value}");
            total2 = aggregated.Max(kvp => kvp.Value);


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 22 ***********************************");
            Thread.Sleep(1000);
        }

        private long GenerateSecret(long seed)
        {
            var res = seed;
            res = ((res * 0b_100_0000) ^ res) % 0b_1_0000_0000_0000_0000_0000_0000;
            res = ((res / 0b_10_0000) ^ res) % 0b_1_0000_0000_0000_0000_0000_0000;
            res = ((res * 0b_1000_0000_0000) ^ res) % 0b_1_0000_0000_0000_0000_0000_0000;
            return res;
        }
    }
}


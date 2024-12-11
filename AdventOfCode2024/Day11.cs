using AdventOfCode2024.Helpers;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    internal class Day11
    {
        const bool TEST = false;
        private List<long> stones;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 11 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day11.txt";
            stones = File.ReadAllText(path).Split(' ').Select(s => long.Parse(s)).ToList();
            total1 = CalculateIterations(stones, 25);
            total2 = CalculateIterations(stones, 75);
            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 11 ***********************************");
            Thread.Sleep(1000);
        }


        private Dictionary<long, (long output1,long? output2)> cache = new Dictionary<long, (long output1, long? output2)>();


        private long CalculateIterations(List<long> stones, int iterations)
        {
            //The key to this puzzle is that the stones order has no impact at all on the result.
            //So we can process the stones by batch by storing at each blink the distinct stone numbers and the number of times they appear
            //This number can they be multiplied by the blink output of each number.
            //The blink output of each individual number can also be cached as to avoid recalculating it every time
            var dic = stones.GroupBy(l => l).ToDictionary(g => g.Key, g => g.LongCount());
            for (int i = 0; i < iterations; i++)
            {
                dic = RunBlink(dic);
            }
            return dic.Select(kvp => kvp.Value).Sum();
        }

        private Dictionary<long, long> RunBlink(Dictionary<long, long> stones)
        {
            var res = new Dictionary<long, long>();
            foreach (var stone in stones.Keys)
            {
                var blinked = GetBlink(stone);
                if (!res.ContainsKey(blinked.output1)) res.Add(blinked.output1, 0);
                res[blinked.output1] += stones[stone];
                if (blinked.output2.HasValue)
                {
                    if (!res.ContainsKey(blinked.output2.Value)) res.Add(blinked.output2.Value, 0);
                    res[blinked.output2.Value] += stones[stone];                    
                }
            }
            return res;
        }

        private (long output1, long? output2) GetBlink(long stone)
        {
            if (cache.ContainsKey(stone))
            {
                return cache[stone];
            }
            long out1;
            long? out2=null;
            if (stone == 0)
            {
                out1 = 1;
            }
            else
            {
                var (l1, l2) = split(stone);
                if (l1.HasValue)
                {
                    out1 = l1.Value;
                    out2 = l2.Value;
                }
                else
                {
                    out1 = stone * 2024;
                }
            }
            cache[stone] = (out1,out2);
            return (out1,out2);
        }

        private (long? l1, long? l2) split(long value)
        {
            if (value / 10 != 0)
            {
                if (value / 100 == 0)
                {
                    return (value / 10, value % 10);
                }
                else
                {
                    if (value / 1_000 == 0)
                    {
                        return (null, null);
                    }
                    else
                    {
                        if (value / 10_000 == 0)
                        {
                            return (value / 100, value % 100);
                        }
                        else
                        {
                            if (value / 100_000 == 0)
                            {
                                return (null, null);
                            }
                            else
                            {
                                if (value / 1_000_000 == 0)
                                {
                                    return (value / 1000, value % 1000);
                                }
                                else
                                {
                                    if (value / 10_000_000 == 0)
                                    {
                                        return (null, null);
                                    }
                                    else
                                    {
                                        if (value / 100_000_000 == 0)
                                        {
                                            return (value / 10_000, value % 10_000);
                                        }
                                        else
                                        {
                                            if (value / 1_000_000_000 == 0)
                                            {
                                                return (null, null);
                                            }
                                            else
                                            {
                                                if (value / 10_000_000_000 == 0)
                                                {
                                                    return (value / 100_000, value % 100_000);
                                                }
                                                else
                                                {
                                                    if (value / 100_000_000_000 == 0)
                                                    {
                                                        return (null, null);
                                                    }
                                                    else
                                                    {
                                                        if (value / 1_000_000_000_000 == 0)
                                                        {
                                                            return (value / 1_000_000, value % 1_000_000);
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("too high");
                                                            return (null, null);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return (null, null);
            }
        }
    }
}


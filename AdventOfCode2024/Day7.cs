using System.Data;

namespace AdventOfCode2024
{
    internal class Day7
    {
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 7 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day7.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var x = ProcessLine(line);
                    total1 += x;
                    if (x > 0)
                    {
                        //No need to recalculate with || since we are OK with just the 2 first operators
                        total2 += x;
                    }
                    else
                    {
                        Console.WriteLine("\tMISSED without concat");
                        x = ProcessLine(line, true);
                        if (x > 0)
                        {
                            total2 += x;
                        }
                        else
                        {
                            Console.WriteLine("\tMISSED");
                        }
                    }
                }
            }


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 7 ***********************************");
            Thread.Sleep(1000);
        }

        private long ProcessLine(string line, bool useConcat = false)
        {
            var colon = line.IndexOf(':');
            var goal = long.Parse(line.Substring(0, colon));
            var values = line.Substring(colon + 2).Split(' ').Select(s => long.Parse(s)).ToList();
            Console.Write($"Goal is {goal}, values are {string.Join(',', values)} {(useConcat ? "(with concat)" : "")}");

            //Shortcut : try some direct calculations
            var lowest = GetLowest(values);
            var highest = GetHighest(values,useConcat);
            Console.Write($"\t result reachable within [{lowest},{highest}]");
            if (goal < lowest || goal > highest)
            {
                Console.WriteLine("\t => GOAL OUT OF REACH !!");
                return 0;
            }
            if (goal == lowest || goal == highest)
            {
                Console.WriteLine("\t => DIRECT FIND !!");
                return goal;
            }

            if (Calculate(goal, values, 0, 0, "+", useConcat)) return goal;
            if (Calculate(goal, values, 0, 0, "*", useConcat)) return goal;
            if (useConcat && Calculate(goal, values, 0, 0, "||", useConcat)) return goal;
            return 0;
        }

        private static long GetHighest(List<long> values, bool useConcat = false)
        {
            return values.Aggregate((i1, i2) =>
            {
                var l = Math.Max((i1 + i2), (i1 * i2));
                if (useConcat)
                {
                    l = Math.Max(l, long.Parse(string.Concat(i1, i2)));
                }
                return l;
            });
        }

        private static long GetLowest(List<long> values)
        {
            return values.Where(i => i != 1).Aggregate((i1, i2) => i1 + i2);
        }

        private bool Calculate(long goal, List<long> values, int currentIndex, long currentResult, string ope, bool useConcat = false)
        {
            if (currentIndex == values.Count)
            {
                if (currentResult == goal) Console.WriteLine("\t GOAL REACHED !");
                return currentResult == goal;
            }
            var curVal = values[currentIndex];
            if (ope == "+") currentResult += curVal;
            if (ope == "*") currentResult *= curVal;
            if (ope == "||") currentResult = long.Parse(string.Concat(currentResult, curVal));
            if (currentResult > goal) return false; //Shortcut : goal is passed, no need to continue on this path
            return Calculate(goal, values, currentIndex + 1, currentResult, "+", useConcat)
                || Calculate(goal, values, currentIndex + 1, currentResult, "*", useConcat)
                || (useConcat && Calculate(goal, values, currentIndex + 1, currentResult, "||", useConcat));
        }
    }
}


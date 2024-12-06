using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Day3
    {
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 3 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = @"Inputs\day3.txt";
            using (var sr = new StreamReader(path, true))
            {
                var enabled = true;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    total1 += ProcessLine(line);
                    total2 += ProcessLine2(line,ref enabled);
                }
            }

            //Print out total result
            Console.WriteLine($"Final result for 1st star is : {total1}");
            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 3 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private long ProcessLine(string line)
        {
            var re = new Regex(@"mul\((?<left>\d{1,3}),(?<right>\d{1,3})\)");
            var matches = re.Matches(line);
            long total = 0;
            foreach (Match m in matches)
            {
                //Console.WriteLine($"Found {m.Value}");
                var left = int.Parse(m.Groups["left"].Value);
                var right = int.Parse(m.Groups["right"].Value);
                total += left * right;
            }
            return total;
        }

        private long ProcessLine2(string line,ref bool enabled)
        {
            //Console.WriteLine($"Starting out with enabled={enabled}");
            var re = new Regex(@"mul\((?<left>\d{1,3}),(?<right>\d{1,3})\)|do\(\)|don't\(\)");
            var matches = re.Matches(line);
            long total = 0;
            foreach (Match m in matches)
            {
                if (m.Value == "do()")
                {
                    enabled = true;
                }
                else if (m.Value == "don't()")
                {
                    enabled = false;
                }
                else
                {
                    if (enabled)
                    {
                        var left = int.Parse(m.Groups["left"].Value);
                        var right = int.Parse(m.Groups["right"].Value);
                        //Console.WriteLine($"\tAdding ({left}*{right})");
                        total += left * right;
                    }
                    else
                    {
                        //Console.WriteLine($"\t{m.Value} is skipped because of DON'T()");
                    }
                }
            }
            return total;
        }
    }
}

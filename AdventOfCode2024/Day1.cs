using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Day1
    {
        private List<int> left = new List<int>();
        private List<int> right = new List<int>();
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 1 !! ##################################\r\n");
            int total1 = 0;
            long total2 = 0;
            var re = new Regex(@"(?<left>\d+)\s+(?<right>\d+)");
            //Read file
            var path = @"Inputs\day1.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    //For each line in file, manage line and retrieve number
                    var m = re.Match(sr.ReadLine());
                    if (m.Success)
                    {
                        left.Add(int.Parse(m.Groups["left"].Value));
                        right.Add(int.Parse(m.Groups["right"].Value));
                    }
                }
            }
            left.Sort();
            right.Sort();
            var dicRight = right.GroupBy(i => i).ToDictionary(g => g.Key, g => g.Count());
            for (int i = 0; i < left.Count; i++)
            {
                total1 += Math.Abs(right[i] - left[i]);
                if (dicRight.ContainsKey(left[i]))
                {
                    total2 += left[i] * dicRight[left[i]];
                }
            }

            //Print out total result
            Console.WriteLine($"Final result for 1st star is : {total1}");
            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 1 ***********************************\r\n");
            Thread.Sleep(1000);
        }
    }
}

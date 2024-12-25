using System.Data;

namespace AdventOfCode2024
{
    internal class Day19
    {
        const bool TEST = false;
        private Dictionary<char,List<string>> towels;
        private List<string> patterns;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 19 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day19.txt";
            var lines = File.ReadAllLines(path);
            towels = lines[0].Split(", ").GroupBy(t => t[0]).ToDictionary(g=>g.Key,g=>g.OrderByDescending(s=>s.Length).ThenBy(s=>s).ToList());
            patterns = lines.Skip(2).ToList();

            foreach(var pattern in patterns)
            {
                //When you are at a given position of the pattern, the number of solutions to finish the pattern will always be the same
                //So we can use a cache to avoid recalculation.
                var positionCache = new Dictionary<int,long>();
                var res = CountAllSolutionsForPattern(pattern, 0,positionCache);
                //Console.WriteLine($"{pattern}: {res} solutions");
                total1 += res>0 ? 1 : 0;
                total2 += res;
            }


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 19 ***********************************");
            Thread.Sleep(1000);
        }

        private long CountAllSolutionsForPattern(string pattern, int position, Dictionary<int, long> positionCache)
        {
            if (position == pattern.Length) return 1; //Found one solution (we reached the end of the pattern)            
            if (!towels.ContainsKey(pattern[position])) return 0; //Quick exit as no towel begins with the first character to match (unlikely)
            var result = 0L;
            foreach (var candidate in towels[pattern[position]])
            {
                if (candidate.Length > pattern.Length - position) continue; //Bad candidate, it's longer than the remaining pattern
                if (pattern.Substring(position, candidate.Length).SequenceEqual(candidate))
                {
                    //We found a suitable candidate, we compute the possible solutions for the remainder of the pattern
                    //First checking the cache to know if we haven't already got the answer.
                    int newPosition = position + candidate.Length;
                    if (!positionCache.ContainsKey(newPosition))
                    {
                        positionCache[newPosition] = CountAllSolutionsForPattern(pattern, newPosition, positionCache);                        
                    }
                    result += positionCache[newPosition];                    
                }
            }
            return result;
        }

    }
}


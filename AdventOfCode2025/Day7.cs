using AdventOfCode2025.Helpers;

namespace AdventOfCode2025
{
    //Replace 7 by day number
    internal class Day7
    {
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 7  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day7.txt";

            var map = MapHelper.LoadCharMap(path);
            var beamLocations = new List<int>() { findStart(map) };
            for (int row = 1; row < map.GetLength(0); row++)
            {
                var newLocations = new List<int>();
                foreach (int bl in beamLocations)
                {
                    char c = map[row, bl];
                    if (c == '.')
                    {
                        map[row, bl] = '|';
                        newLocations.Add(bl);
                    }
                    else if (c == '^')
                    {
                        total1++;
                        if (bl > 0 && map[row, bl - 1] == '.')
                        {
                            map[row, bl - 1] = '|';
                            newLocations.Add(bl - 1);
                        }
                        if (bl < map.GetLength(0) - 1 && map[row, bl + 1] == '.')
                        {
                            map[row, bl + 1] = '|';
                            newLocations.Add(bl + 1);
                        }
                    }
                }
                beamLocations = newLocations;
            }            

            ////////////////////////////////////////:
            map = MapHelper.LoadCharMap(path); //Reset map
            total2 = timelinesCount(map, 1, findStart(map)); //Launch recursive count

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 7 ***********************************");
            Thread.Sleep(1000);
        }

        private static int findStart(char[,] map)
        {
            for (int i = 0; i < map.GetLength(1); i++)
            {
                if (map[0, i] == 'S')
                {
                    return i;
                }
            }
            return -1;
        }

        private Dictionary<(int row,int col),long> _cache = new Dictionary<(int row, int col), long> ();
        private long timelinesCount(char[,] map, int row, int col)
        {
            //for a given position, the subtree is always the same, so we can cache it to speed calculation.
            if(_cache.ContainsKey((row,col))) return _cache[(row,col)];
            //Exit condition when the bottom of the map is reached.
            if (row == map.GetLength(0)) return 1;
            var c = map[row, col];
            long result = 0;
            if(c == '.')
            {
                result = timelinesCount(map, row + 1, col);
            }
            else if (c == '^')
            {
                if (col > 0) result += timelinesCount(map, row + 1, col - 1);
                if (col < map.GetLength(1)-1) result += timelinesCount(map, row + 1, col + 1);
            }
            _cache[(row, col)] = result;
            return result;
        }
    }
}


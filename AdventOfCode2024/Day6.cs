using AdventOfCode2024.Helpers;
using System.Drawing;

namespace AdventOfCode2024
{
    internal class Day6
    {

        private char[,] map;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 6 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = @"Inputs\day6.txt";
            map = MapHelper.LoadCharMap(path);
            //Find starting point
            Point start=Point.Empty;
            map.ForEachIndex(p =>
            {
                if (IsGuard(map.Get(p)))
                {
                    start= p;
                }
            });
            var curDirection = GetDirection(map.Get(start));
            var curPos = start;
            var visited = new HashSet<Point>();
            var visitedWithDir = new HashSet<(Point,dirs)>();
            var alreadyTestedPoints  = new HashSet<Point>();
            var possibleBlocks = new HashSet<Point>();
            var allIWant = new AllIWantIsProgress();
            while (true)
            {
                visited.Add(curPos);
                allIWant.Progress(1);
                visitedWithDir.Add((curPos, curDirection));
                var newPos = curPos.Move(curDirection);
                if (map.IsOutOfBound(newPos)) break;
                
                if (map.Get(newPos) == '#')
                {
                    curDirection = NavigationHelper.TurnRight(curDirection);
                    map.Set(curPos, '+');
                }
                else
                {
                    curPos = newPos;
                    map.Set(curPos, GetPathChar(curDirection));
                }


                //The guard get stuck in the loop if the block puts us on a path where the position and direction have already been visited.                
                var testedPos = curPos.Move(curDirection);
                if(!map.IsOutOfBound(testedPos) && map.Get(testedPos) != '#')
                {
                    //We cannot test a position a second time because it would cause a time paradox
                    //(if there was a block there then we would not be where we are now)
                    if (alreadyTestedPoints.Contains(testedPos)) continue;
                    alreadyTestedPoints.Add(testedPos);
                    
                    //Console.Write($"Testing block in {testedPos}");
                    var testPath = new List<(Point,dirs)>();
                    var hypVisitedWithDir = new HashSet<(Point,dirs)>(visitedWithDir);
                    //var hypMap = map.Clone() as char[,];
                    var hypDir = NavigationHelper.TurnRight(curDirection);
                    var hypLoc = curPos;
                    //if (!canUseCache) Console.Write("\t cannot use cache");
                    while (true)
                    {
                        var newPos2 = hypLoc.Move(hypDir);
                        if (map.IsOutOfBound(newPos2))
                        {
                            //If we exit the map, then the test is failed.
                            break;
                        }

                        if (map.Get(newPos2) == '#' || newPos2==testedPos)
                        {
                            //If we bump again on the tested position during the test, we turn.
                            hypDir = NavigationHelper.TurnRight(hypDir);
                            //hypMap.Set(hypLoc, '*');
                            //We clear the test path because it is invalid for the cache (depends on temporary block)
                            if (newPos2 == testedPos)
                            {
                                //Console.Write("\tre-bumping on temp block");
                                testPath.Clear();
                            }
                            continue;
                        }

                        //Else we move forward
                        hypLoc = newPos2;
                        //hypMap.Set(hypLoc, 'x');
                        testPath.Add((hypLoc, hypDir));

                        //If we already visited the point with the same direction, then we are on a loop, the test is successful.
                        if (hypVisitedWithDir.Contains((hypLoc, hypDir)))
                        {
                            //Console.WriteLine("\tLoop success");
                            possibleBlocks.Add(testedPos);
                            //hypMap.Set(testedPos, 'O');
                            //hypMap.Print();
                            break;
                        }
                        
                        //Else we log the current step because hypothetical path also counts for building a loop.
                        hypVisitedWithDir.Add((hypLoc, hypDir));
                    }                    
                }
            }
            map.Print();
            total1 = visited.Count;
            total2 = possibleBlocks.Count;
           

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 6 ***********************************");
            Thread.Sleep(1000);
        }

        private char GetPathChar(dirs d)
        {
            switch (d)
            {
                case dirs.north: 
                case dirs.south: return '|';
                case dirs.east: 
                case dirs.west: return '-';
                default: return 'o';
            }
        }

        private dirs GetDirection(char c)
        {
            switch (c)
            {
                case '>': return dirs.east;
                case '<': return dirs.west;
                case 'v': return dirs.south;
                case '^': return dirs.north;
                default: return dirs.north;
            }
        }

        private bool IsGuard(char c)
        {
            char[] guards = { '^', '<', '>', 'v' };
            return guards.Contains(c);
        }
    }
}


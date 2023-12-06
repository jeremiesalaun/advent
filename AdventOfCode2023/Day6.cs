using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day6
    {
        private class race
        {
            public race() { }

            public race(long Time, long CurrentRecord)
            {
                this.Time = Time; this.CurrentRecord = CurrentRecord;
            }
            public long Time { get; set; }
            public long CurrentRecord { get; set; }

            public long GetDistance(long holdTime)
            {
                if (this.Time <= holdTime) return 0;
                return (this.Time - holdTime) * holdTime; //remaining time * speed
            }

            public bool CanWin(long holdTime)
            {
                return GetDistance(holdTime) > this.CurrentRecord;
            }
        }
        public void Run()
        {
            //Inputs
            var races = new race[] {
                new race(44, 208),
                new race(80, 1581),
                new race(65, 1050),
                new race(72, 1102)
            };
            //******** 1st STAR **************
            long result1 = 1;
            foreach (var race in races)
            {
                int nbWin = 0;
                for (int holdTime = 0; holdTime < race.Time; holdTime++)
                {
                    nbWin += race.CanWin(holdTime) ? 1 : 0;
                }
                result1 *= nbWin;
            }
            Console.WriteLine($"Final result for 1st star is : {result1}");

            //******** 2nd STAR **************
            //var longRace = new race(7, 9);
            //var longRace = new race(30, 200);
            var longRace = new race(44806572, 208158110501102);
            long result2 = GetNbWin(longRace, 0, longRace.Time, true);
            Console.WriteLine($"Final result for 2nd star is : {result2}");
        }

        private long GetNbWin(race r, long curMin, long curMax, bool isLeft)
        {
            long result = 0;
            bool canWinLeft = r.CanWin(curMin);
            bool canWinRight = r.CanWin(curMax);
            if (canWinLeft && canWinRight)
            {
                result = curMax - curMin + 1;
                Console.WriteLine($"V on all interval [{curMin},{curMax}]");
            }
            else
            {
                long curTime = (curMin + curMax) / 2;
                if (curTime == curMin)
                {
                    result = r.CanWin(curTime) ? 1 : 0;
                }
                else if (r.CanWin(curTime))
                {
                    Console.WriteLine($"V {curTime}, launching on left [{curTime + 1},{curMax}]");
                    result += 1;
                    if (canWinLeft)
                    {
                        result += curTime - curMin;
                    }
                    else
                    {
                        Console.WriteLine($"V {curTime}, launching on left [{curMin},{curTime - 1}]");
                        result += GetNbWin(r, curMin, curTime - 1, true);
                    }

                    if (canWinRight)
                    {
                        result += curMax - curTime;
                    }
                    else
                    {
                        Console.WriteLine($"V {curTime}, launching on right [{curTime + 1},{curMax}]");
                        result += GetNbWin(r, curTime + 1, curMax, false);
                    }                    
                }
                else if (isLeft)
                {
                    Console.WriteLine($"X {curTime}, launching on left [{curTime + 1},{curMax}]");
                    result += GetNbWin(r, curTime + 1, curMax, true);
                }
                else
                {
                    Console.WriteLine($"X {curTime}, launching on right [{curMin},{curTime - 1}]");
                    result += GetNbWin(r, curMin, curTime - 1, false);
                }
            }
            Console.WriteLine($"interval [{curMin},{curMax}] = {result} win");
            return result;
        }


    }
}

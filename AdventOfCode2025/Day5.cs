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
    //Replace 5 by day number
    internal class Day5
    {
        const bool TEST = false;

        private class freshRange
        {
            public long Start {  get; set; }
            public long End { get; set; }
            public bool IsFresh(long productId)
            {
                return (productId >= Start && productId <= End);
            }

            public bool IsOverlapping(freshRange r)
            {
                return (this.Start <= r.End && this.End >= r.Start);
            }

            public void FusionWith(freshRange r)
            {
                this.Start = Math.Min(this.Start, r.Start);
                this.End = Math.Max(this.End, r.End);
            }

            public long Length()
            {
                return 1 + this.End - this.Start;
            }

            public static int Comparator(freshRange r1,freshRange r2)
            {
                return r2.Start == r1.Start ? (r2.End == r1.End ? 0 : r2.End > r1.End ? 1 : -1) : r2.Start > r1.Start ? 1 : -1;
            }
        }


        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 5  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day5.txt";
            var ranges = new List<freshRange>();
            var productIds = new List<long>();
            foreach(var l in File.ReadAllLines(path))
            {
                if (l.Length == 0) continue;
                if (l.Contains('-'))
                {
                    var a = l.Split('-');
                    ranges.Add(new freshRange() { Start = long.Parse(a[0]), End = long.Parse(a[1])});
                }
                else
                {
                    productIds.Add(long.Parse(l));
                }
            }

            foreach(var pid in productIds)
            {
                foreach(var range in ranges)
                {
                    if (range.IsFresh(pid))
                    {
                        total1++;
                        break;
                    }
                }
            }

            //The idea is to make sure to not count two times a productId.
            //This is done by deoverlapping the ranges and building a new list of distinct ranges.
            //Once we have them, we just have to count their length.

            //First we sort the ranges so that we are sure that overlapping ranges are adjacent in the list
            ranges.Sort(freshRange.Comparator);
            var distinctRanges = new List<freshRange>();
            var curRange = ranges[0];
            for(int i=1; i<ranges.Count; i++)
            {
                if (curRange.IsOverlapping(ranges[i]))
                {
                    //As long as adjacent ranges are overlapping, we combine them into a single range
                    curRange.FusionWith(ranges[i]);
                }
                else
                {
                    //We found a gap between 2 adjacent ranges, we store the range that we have been building and start fresh with the next one.
                    distinctRanges.Add(curRange);
                    curRange=ranges[i];
                }
            }
            distinctRanges.Add(curRange); //Don't forget the last range that we were building

            total2 = distinctRanges.Select(r => r.Length()).Sum();



            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 5 ***********************************");
            Thread.Sleep(1000);
        }

    }
}


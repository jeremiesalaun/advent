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
    //Replace 2 by day number
    internal class Day2
    {
        private class IdRange
        {
            public static IdRange FromString(string s)
            {
                var a = s.Split('-');
                return new IdRange
                {
                    Start = long.Parse(a[0]),
                    End = long.Parse(a[1])
                };
            }
            
            public static int DigitCount(long input)
            {
                return input.ToString().Length;
            }

            public static bool IsEvenDigitNumber(long input)
            {
                return DigitCount(input)%2==0;
            }

            public long Start { get; set; }
            public long End { get; set; }

        }

        private HashSet<long> alreadyFound = new HashSet<long>();

        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 2  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day2.txt";
            var ranges = File.ReadAllText(path).Split(',').Select(s => IdRange.FromString(s)).ToList();
            Console.WriteLine("---------- FIRST STAR --------------");
            alreadyFound.Clear();
            foreach (var r in ranges)
            {
                total1 += SumFalseIdTwice(r);
            }
            Console.WriteLine("---------- SECOND STAR --------------");
            alreadyFound.Clear();
            foreach (var r in ranges)
            {
                total2 += SumFalseIds(r);
            }


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 2 ***********************************");
            Thread.Sleep(1000);
        }

        private long SumFalseIdTwice(IdRange r)
        {
            int repeatCount = 2;
            //Console.WriteLine($"Testing range [{r.Start}-{r.End}]");
            long sum = 0;
            var dcs = IdRange.DigitCount(r.Start);
            var dce = IdRange.DigitCount(r.End);
            for(int digitCount = dcs; digitCount <= dce; digitCount++)
            {
                if (digitCount % repeatCount != 0) continue;
                var rangeStart = long.Parse("1" + new string('0', digitCount - 1));
                var rangeEnd = long.Parse(new string('9', digitCount));
                rangeStart = Math.Max(rangeStart, r.Start);
                rangeEnd = Math.Min(rangeEnd, r.End);
                //Console.WriteLine($"Testing subrange [{rangeStart}-{rangeEnd}]");
                sum +=SumFalseIdsForGivenLength(digitCount,repeatCount,rangeStart,rangeEnd);
            }
            return sum;
        }

        private long SumFalseIds(IdRange r)
        {
            //Console.WriteLine($"Testing range [{r.Start}-{r.End}]");
            long sum = 0;
            var dcs = IdRange.DigitCount(r.Start);
            var dce = IdRange.DigitCount(r.End);
            for(int repeatCount=2; repeatCount<=dce; repeatCount++) { 
                for (int digitCount = dcs; digitCount <= dce; digitCount++)
                {
                    if (digitCount<repeatCount || digitCount % repeatCount != 0) continue;
                    var rangeStart = long.Parse("1" + new string('0', digitCount - 1));
                    var rangeEnd = long.Parse(new string('9', digitCount));
                    rangeStart = Math.Max(rangeStart, r.Start);
                    rangeEnd = Math.Min(rangeEnd, r.End);
                    //Console.WriteLine($"Testing subrange [{rangeStart}-{rangeEnd}] with repeat count {repeatCount}");
                    sum += SumFalseIdsForGivenLength(digitCount, repeatCount, rangeStart, rangeEnd);
                }
            }
            return sum;
        }

        private long SumFalseIdsForGivenLength(int digitCount,int repeatCount, long rangeStart,long rangeEnd)
        {
            long sum=0;
            //It can only work with multiples of repeatCount
            if (digitCount % repeatCount != 0) return 0;

            var startRange = long.Parse(rangeStart.ToString().Substring(0, digitCount / repeatCount));
            var endRange = long.Parse(rangeEnd.ToString().Substring(0, digitCount / repeatCount));
            for (var i = startRange; i <= endRange; i++)
            {
                var falseId = long.Parse(RepeatString(i.ToString(), repeatCount));
                if (falseId >= rangeStart && falseId <= rangeEnd && !alreadyFound.Contains(falseId))
                {
                    //Console.WriteLine($"Found falseId {falseId}");
                    alreadyFound.Add(falseId);
                    sum+=falseId;
                }
            }

            return sum;
        }

        private static string RepeatString(string val, int count)
        {
            var sb = new StringBuilder();
            for(int i = 0; i < count; i++)
            {
                sb.Append(val);
            }
            return sb.ToString();
        }
    }
}


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
    //Replace 3 by day number
    internal class Day3
    {
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 3  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day3.txt";
            var banks = File.ReadAllLines(path).Select(l => l.Select(c => int.Parse(c.ToString())).ToList()).ToList();
            foreach(var bank in banks)
            {
                total1 += MaxJolt2(bank);
            }

            foreach (var bank in banks)
            {
                total2 += MaxJolt12(bank);
            }


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 3 ***********************************");
            Thread.Sleep(1000);
        }

        private long MaxJolt12(List<int> bank)
        {
            long result=0;
            int previousDigitIndex = -1;
            long tempMax = 0;
            for(int k = 11; k >= 0; k--)
            {
                tempMax = 0;
                for (int i = previousDigitIndex+1; i < bank.Count - k; i++)
                {
                    if (bank[i] > tempMax)
                    {
                        previousDigitIndex = i;
                        tempMax = bank[i];
                    }
                }
                result += tempMax * (long)Math.Pow(10,k);
            }
            Console.WriteLine(result);
            return result;
        }

        private long MaxJolt2(List<int> bank)
        {
            long result;
            int firstDigitIndex=0;
            int tempMax=0;
            for (int i = 0; i < bank.Count-1; i++)
            {
                if (bank[i] > tempMax)
                {
                    firstDigitIndex = i;
                    tempMax = bank[i];
                }
            }
            result = tempMax * 10;
            tempMax = 0;
            for (int i = firstDigitIndex+1; i < bank.Count; i++)
            {
                if (bank[i] > tempMax)
                {
                    tempMax = bank[i];
                }
            }
            result += tempMax;
            return result;
        }
    }
}


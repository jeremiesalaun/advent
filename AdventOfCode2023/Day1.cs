using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day1
    {
        public void Run()
        {
            int total=0;
            //Read file
            var path = @"Inputs\day1.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    //For each line in file, manage line and retrieve number
                    //Add results
                    total += ManageLine(sr.ReadLine());
                }
            }
            //Print out total result
            Console.WriteLine($"Final result is : {total}");
        }

        private int ManageLine(string line)
        {
            var refirst = new Regex(@"(?<digit>\d|one|two|three|four|five|six|seven|eight|nine)");
            var relast = new Regex(@".*(?<lastdigit>\d|one|two|three|four|five|six|seven|eight|nine).*");
            if (line == null) return 0;
            line = line.ToLower();
            var mfirst = refirst.Match(line);
            var mlast = relast.Match(line);
            int result = 0;
            if (mfirst.Success && mlast.Success)
            {
                result = ParseDigit(mfirst.Groups["digit"].Value) * 10 + ParseDigit(mlast.Groups["lastdigit"].Value);
            }
            Console.WriteLine($"Digit for line {line} is {result}");
            return result;
        }

        private int ParseDigit(string value)
        {
            int result;
            if(string.IsNullOrWhiteSpace(value)) return 0;
            if (int.TryParse(value, out result)) return result;
            switch (value)
            {
                case "one": result = 1; break; 
                case "two": result = 2; break;
                case "three": result = 3; break;                    
                case "four": result = 4; break;
                case "five": result = 5; break;
                case "six": result = 6; break;
                case "seven": result = 7; break;
                case "eight": result = 8; break;
                case "nine": result = 9; break;
            }
            return result;
        }
    }
}

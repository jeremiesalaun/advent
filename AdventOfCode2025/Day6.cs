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
    //Replace 6 by day number
    internal class Day6
    {
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 6  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day6.txt";
            
            
            var operands = new List<long[]>();
            string[] operators = new string[0] ;
            foreach(var l in File.ReadAllLines(path))
            {
                if(l.Contains("+"))
                {
                    operators =l.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    operands.Add(l.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToArray());
                }
            }

            for(int i=0; i<operators.Length; i++)
            {
                long result = operators[i] == "+" ? 0 : 1;
                for(int j = 0; j < operands.Count; j++)
                {
                    if (operators[i] == "+")
                    {
                        result = result + operands[j][i];
                    } else
                    {
                        result = result * operands[j][i];
                    }
                }
                total1 += result;
            }


            ///////////////////////////////////////////////////////////////////////////////
            var map = MapHelper.LoadCharMap(path);
            var operow = map.GetLength(0) - 1;
            char op=' ';
            var operands2 = new List<long>();
            for(int col=0;col<map.GetLength(1);col++)
            {
                //When we reach a new operator in the bottom row then we can perform the previous operation
                //with all operands we got this far.
                //Another option would be to detect the empty column
                //Another option, easier for the mind, would have been to transpose the matrix and read it row by row
                if(map[operow,col]!=' ')
                {
                    //perform operation
                    if (operands2.Any()) total2 += performOp(operands2, op);
                    //reset state
                    operands2.Clear();
                    op = map[operow,col];
                }
                var sb = new StringBuilder();
                for(int j = 0; j < operow; j++)
                {
                    if(map[j, col]!=' ') sb.Append(map[j, col]);
                }
                if(sb.Length > 0) operands2.Add(long.Parse(sb.ToString()));
            }
            //perform last operation
            total2 += performOp(operands2, op);

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 6 ***********************************");
            Thread.Sleep(1000);
        }

        private long performOp(List<long> operands, char @operator)
        {
            long result = @operator == '+' ? 0 : 1;
            for (int j = 0; j < operands.Count; j++)
            {
                if (@operator == '+')
                {
                    result = result + operands[j];
                }
                else
                {
                    result = result * operands[j];
                }
            }
            return result;
        }
    }
}


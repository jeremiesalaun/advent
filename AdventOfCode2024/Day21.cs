using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    //Replace 21 by day number
    internal class Day21
    {
        const bool TEST = true;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 21 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day21.txt";


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 21 ***********************************");
            Thread.Sleep(1000);
        }

    }
}


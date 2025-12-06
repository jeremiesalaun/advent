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
    //Replace 7 by day number
    internal class Day7
    {
        const bool TEST = true;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 7  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day7.txt";


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 7 ***********************************");
            Thread.Sleep(1000);
        }

    }
}


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
    //Replace _0_ by day number
    internal class Day_0_
    {

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY _0_ !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = @"Inputs\day_0_.txt";


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY _0_ ***********************************");
            Thread.Sleep(1000);
        }

    }
}


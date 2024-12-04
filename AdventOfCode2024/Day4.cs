using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    internal class Day4
    {
        private char[,] map;
        private char[] xmas = { 'X', 'M', 'A', 'S' };
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 4 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            map = MapHelper.LoadCharMap(@"Inputs\day4.txt");
            map.ForEachIndex(p =>
            {
                switch (map[p.X, p.Y])
                {
                    case 'X':
                        //Test if X is the start of one or more XMAS.
                        total1 += FindXMas(p.X, p.Y);
                        break;
                    case 'A':
                        //Test if the A is the center of an X-MAS.
                        total2 += FindX_Mas(p.X, p.Y);
                        break;
                }
            });

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 4 ***********************************");
            Thread.Sleep(1000);
        }

        private int FindXMas(int i, int j)
        {
            int res = 0;
            //Iterate over every direction
            foreach (int incI in new int[] { -1, 0, 1 })
            {
                foreach (int incJ in new int[] { -1, 0, 1 })
                {
                    res += FindXMas(i, j, incI, incJ) ? 1 : 0;
                }
            }
            return res;
        }

        private bool FindXMas(int i, int j, int incI, int incJ)
        {
            if (incI == 0 && incJ == 0) return false;
            foreach (var letter in xmas)
            {
                if (map.IsOutOfBound(i, j)) return false;
                if (map[i, j] != letter) return false;
                i += incI;
                j += incJ;
            }
            return true;
        }
        private long FindX_Mas(int i, int j)
        {
            //Out of bound check
            if (map.IsOutOfBound(i, j, 1)) return 0;
            //Get all for points of the X around the A.
            var NO = map[i - 1, j - 1];
            var NE = map[i - 1, j + 1];
            var SE = map[i + 1, j + 1];
            var SO = map[i + 1, j - 1];
            switch ((NO, NE, SE, SO))
            {
                //There are only 4 possible patterns to get a X-MAS
                case ('M', 'M', 'S', 'S'):
                case ('S', 'M', 'M', 'S'):
                case ('S', 'S', 'M', 'M'):
                case ('M', 'S', 'S', 'M'): return 1;
                default: return 0;
            }
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day11
    {
        public void Run()
        {
            var galaxies = new List<Point>();
            //Read and parse file
            var inputpath = @"Inputs\day11.txt";
            var notExpandedRows = new HashSet<int>();
            var notExpandedCols = new HashSet<int>();
            using (var sr = new StreamReader(inputpath, true))
            {
                int row = 0;
                while (!sr.EndOfStream)
                {
                    var l = sr.ReadLine().ToCharArray();
                    for (int col = 0; col < l.Length; col++)
                    {
                        if (l[col] == '#')
                        {
                            galaxies.Add(new Point(row, col));
                            notExpandedRows.Add(row);
                            notExpandedCols.Add(col);
                        }
                    }
                    row++;
                }
            }
            long total1 = 0;
            long total2 = 0;
            for(int i = 0; i < galaxies.Count-1; i++)
            {
                var g1 = galaxies[i];
                for(int j = i+1; j < galaxies.Count; j++)
                {
                    var g2 = galaxies[j];
                    var dist1= Math.Abs(Expand(g2.X,notExpandedRows,2)-Expand(g1.X,notExpandedRows,2))
                            +Math.Abs(Expand(g2.Y,notExpandedCols,2)-Expand(g1.Y,notExpandedCols,2));
                    total1 += dist1;
                    var dist2 = Math.Abs(Expand(g2.X, notExpandedRows, 1_000_000) - Expand(g1.X, notExpandedRows, 1_000_000))
                            + Math.Abs(Expand(g2.Y, notExpandedCols, 1_000_000) - Expand(g1.Y, notExpandedCols, 1_000_000));
                    total2 += dist2;
                }
            }
            Console.WriteLine($"Final result for 1st star is : {total1}");
            Console.WriteLine($"Final result for 2nd star is : {total2}");
        }

        private int Expand(int value,HashSet<int> notExpandedValues,int expansion)
        {
            var nbNotExp = notExpandedValues.Where(x => x < value).Count();
            return nbNotExp + expansion * (value - nbNotExp);
        }
    }
}

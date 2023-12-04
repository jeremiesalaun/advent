using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day3
    {
        private class row
        {
            public int rowindex { get; set; }
            public int rowlength { get; set; }
            public List<symbol> symbols { get; set; } = [];
            public List<number> numbers { get; set; } = [];
        }
        private class symbol
        {
            public char character { get; set; }
            public int row { get; set; }
            public int column { get; set; }
            public bool isgear {  get; set; }
            public List<int> numbers { get; set; } = [];
        }
        private class number
        {
            public int value { get; set; }
            public int row { get; set; }
            public int colstart { get; set; }
            public int colend { get; set; }
        }

        public void Run()
        {
            var rows = new List<row>();
            //Read and parse file
            var path = @"Inputs\day3.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    rows.Add(ParseLine(sr.ReadLine(), rows.Count));
                }
            }
            int total = 0;
            //Process rows
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                foreach (var n in r.numbers)
                {
                    var rmin = Math.Max(0, n.row - 1);
                    var rmax = Math.Min(rows.Count - 1, n.row + 1);
                    var cmin = Math.Max(0, n.colstart - 1);
                    var cmax = Math.Min(r.rowlength - 1, n.colend + 1);
                    for (int j = rmin; j <= rmax; j++)
                    {
                        foreach(var s in rows[j].symbols.Where(s => s.character=='*' &&  s.column >= cmin && s.column <= cmax).ToList())
                        {
                            s.numbers.Add(n.value);
                            s.isgear = (s.numbers.Count==2);
                        }
                    }
                }
            }
            //Calculate total power
            foreach(var g in rows.SelectMany(r => r.symbols).Where(s => s.isgear))
            {
                Console.WriteLine($"The gear at {g.row}:{g.column} has the numbers {g.numbers[0]} and {g.numbers[1]}");
                total += g.numbers[0] * g.numbers[1];
            }
            //Print out total result
            Console.WriteLine($"Final result is : {total}");
        }

        private row ParseLine(string? line, int rowindex)
        {
            if (line == null) return null;
            var result = new row() { rowindex = rowindex, rowlength = line.Length };
            var chars = line.ToCharArray();
            StringBuilder curnumber = null;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '.')
                {
                    if (curnumber != null)
                    {
                        result.numbers.Add(new number()
                        {
                            value = int.Parse(curnumber.ToString()),
                            row = rowindex,
                            colend = i - 1,
                            colstart = i - curnumber.Length
                        });
                        curnumber = null;
                    }
                }
                else if (chars[i] >= '0' && chars[i] <= '9')
                {
                    if (curnumber == null)
                    {
                        curnumber = new StringBuilder();
                    }
                    curnumber.Append(chars[i]);
                }
                else
                {
                    if (curnumber != null)
                    {
                        result.numbers.Add(new number()
                        {
                            value = int.Parse(curnumber.ToString()),
                            row = rowindex,
                            colend = i - 1,
                            colstart = i - curnumber.Length
                        });
                        curnumber = null;
                    }
                    result.symbols.Add(new symbol() { row = rowindex, column = i, character = chars[i] });
                }
            }
            if (curnumber != null)
            {
                result.numbers.Add(new number()
                {
                    value = int.Parse(curnumber.ToString()),
                    row = rowindex,
                    colend = chars.Length - 1,
                    colstart = chars.Length - curnumber.Length
                });
            }
            return result;
        }
    }
}

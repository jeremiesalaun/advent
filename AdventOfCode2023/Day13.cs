using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day13
    {
        private class pattern
        {
            public char[,] map { get; set; }
            public int rowcount { get; set; }
            public int colcount { get; set; }
            public int hmirror { get; set; }
            public int vmirror { get; set; }
            public bool isFixed { get; set; }
            public Point? smudge { get; set; }

            internal void FindHMirror()
            {
                for (int i = 0; i < rowcount - 1; i++)
                {
                    var success = true;
                    for (int k = 0; k < i + 1; k++)
                    {
                        if (i + k + 1 >= rowcount)
                        {
                            break;
                        }
                        for (int j = 0; j < colcount; j++)
                        {
                            if (map[i - k, j] != map[i + k + 1, j])
                            {
                                success = false; break;
                            }
                        }
                        if (!success) break;
                    }
                    if (success)
                    {
                        this.hmirror = i + 1;
                        break;
                    }
                }
            }

            internal void FindHMirrorWithSmudge()
            {
                for (int i = 0; i < rowcount - 1; i++)
                {
                    var success = true;
                    var wasFixed = isFixed;
                    for (int k = 0; k < i + 1; k++)
                    {
                        if (i + k + 1 >= rowcount)
                        {
                            break;
                        }
                        int smudge = -1;
                        for (int j = 0; j < colcount; j++)
                        {
                            if (map[i - k, j] != map[i + k + 1, j])
                            {
                                if (!isFixed && smudge < 0)
                                {
                                    smudge = j;
                                }
                                else
                                {
                                    success = false; break;
                                }
                            }
                        }
                        if (success && smudge >= 0 && !isFixed)
                        {
                            Console.WriteLine($"Fixing smudge at [{i - k},{smudge}]");
                            map[i - k, smudge] = map[i + k + 1, smudge];
                            this.smudge = new Point(i - k, smudge);
                            isFixed = true;
                        }
                        if (!success) break;
                    }
                    if (!success && !wasFixed && isFixed)
                    {
                        //In the end the fix did not work, so we undo it
                        Console.WriteLine($"Undo smudge fix at [{smudge.Value.X},{smudge.Value.Y}]");
                        this.map[smudge.Value.X, smudge.Value.Y] = this.map[smudge.Value.X, smudge.Value.Y] == '#' ? '.' : '#';
                        this.smudge = null;
                        this.isFixed = false;
                    }
                    else if (success && isFixed)
                    {
                        this.hmirror = i + 1;
                        break;
                    }
                }
            }

            internal void FindVMirror()
            {
                for (int j = 0; j < colcount - 1; j++)
                {
                    var success = true;
                    for (int k = 0; k < j + 1; k++)
                    {
                        if (j + k + 1 >= colcount)
                        {
                            break;
                        }
                        for (int i = 0; i < rowcount; i++)
                        {
                            if (map[i, j - k] != map[i, j + k + 1])
                            {
                                success = false; break;
                            }
                        }
                        if (!success) break;
                    }
                    if (success)
                    {
                        this.vmirror = j + 1;
                        break;
                    }
                }
            }

            internal void FindVMirrorWithSmudge()
            {
                for (int j = 0; j < colcount - 1; j++)
                {
                    var success = true;
                    var wasFixed = isFixed;
                    for (int k = 0; k < j + 1; k++)
                    {
                        if (j + k + 1 >= colcount)
                        {
                            break;
                        }
                        int smudge = -1;
                        for (int i = 0; i < rowcount; i++)
                        {
                            if (map[i, j - k] != map[i, j + k + 1])
                            {
                                if (!isFixed && smudge < 0)
                                {
                                    smudge = i;
                                }
                                else
                                {
                                    success = false; break;
                                }
                            }
                        }
                        if (success && smudge >= 0 && !isFixed)
                        {
                            Console.WriteLine($"Fixing smudge at [{smudge},{j - k}]");
                            map[smudge, j - k] = map[smudge, j + k + 1];
                            this.smudge = new Point(smudge, j - k);
                            isFixed = true;
                        }
                        if (!success) break;
                    }
                    if (!success && !wasFixed && isFixed)
                    {
                        //In the end the fix did not work, so we undo it
                        Console.WriteLine($"Undo smudge fix at [{smudge.Value.X},{smudge.Value.Y}]");
                        this.map[smudge.Value.X, smudge.Value.Y] = this.map[smudge.Value.X, smudge.Value.Y] == '#' ? '.' : '#';
                        this.smudge = null;
                        this.isFixed = false;
                    }
                    else if (success && isFixed)
                    {
                        this.vmirror = j + 1;
                        break;
                    }
                }
            }

            public void Print()
            {
                for (int i = 0; i < rowcount; i++)
                {
                    for (int j = 0; j < colcount; j++)
                    {
                        if (i < hmirror)
                        {
                            if (j < vmirror)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                        }
                        else
                        {
                            if (j < vmirror)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                        }
                        if (isFixed && smudge.Value.Equals(new Point(i, j)))
                        {
                            Console.BackgroundColor = ConsoleColor.Magenta;
                        }
                        Console.Write(map[i, j]);
                        Console.ResetColor();
                    }
                    Console.Write(Environment.NewLine);
                }
            }

            internal void Reset()
            {
                this.vmirror = 0;
                this.hmirror = 0;
                this.smudge = null;
                this.isFixed = false;
            }
        }

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 13 !! ##################################\r\n");
            var patterns = new List<pattern>();
            //Read and parse file
            var inputpath = @"Inputs\day13.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                var rows = new List<string>();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Length == 0)
                    {
                        patterns.Add(BuildPattern(rows));
                        rows.Clear();
                    }
                    else
                    {
                        rows.Add(line);
                    }
                }
                if (rows.Count > 0) patterns.Add(BuildPattern(rows)); ;
            }

            long total1 = 0;
            foreach (var p in patterns)
            {
                p.FindHMirror();
                if (p.hmirror == 0)
                {
                    p.FindVMirror();
                }
                //Console.WriteLine($"Mirrors found at {p.hmirror} row and {p.vmirror} col");
                total1 += p.vmirror + 100 * p.hmirror;
            }

            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;
            int i = 0;
            foreach (var p in patterns)
            {
                p.Reset();
                Console.WriteLine($"######## PATTERN NUMBER {i} ##########");
                p.FindVMirrorWithSmudge();
                if (!p.isFixed)
                {
                    p.FindHMirrorWithSmudge();
                }
                Console.WriteLine($"Mirrors found at {p.hmirror} row and {p.vmirror} col");
                p.Print();
                total2 += p.vmirror + 100 * p.hmirror;
                i++;
            }
            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 13 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private pattern BuildPattern(List<string> rows)
        {
            var r = new pattern();
            r.rowcount = rows.Count;
            r.colcount = rows[0].Length;
            r.map = new char[r.rowcount, r.colcount];
            for (int i = 0; i < r.rowcount; i++)
            {
                var line = rows[i].ToCharArray();
                for (int j = 0; j < r.colcount; j++)
                {
                    r.map[i, j] = line[j];
                }
            }
            return r;
        }
    }
}

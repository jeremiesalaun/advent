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
using System.Windows.Markup;

namespace AdventOfCode2023
{
    internal class Day15
    {
        private class Lens
        {
            public string Label { get; set; }
            public int Length { get; set; }
            public Lens(string label, int length)
            {
                Label = label;
                Length = length;
            }
        }
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 15 !! ##################################\r\n");
            string[] inputs;
            //Read and parse file
            var inputpath = @"Inputs\day15.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                var line = sr.ReadLine();
                inputs = line.Split(',').ToArray();
            }
            long total1 = 0;

            var hashed = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                hashed[i] = HASH(inputs[i]);
                total1 += hashed[i];
            }
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;
            var re = new Regex(@"(?<label>[a-z]+)((?<remove>-)|=(?<flength>\d+))");
            var boxes = new Dictionary<int, (List<Lens> lenses, List<string> operations)>();
            for (int i = 0; i < inputs.Length; i++)
            {
                var ope = inputs[i];
                var m = re.Match(ope);
                if (m.Success)
                {
                    var label = m.Groups["label"].Value;
                    int key = HASH(label);
                    var isRemove = m.Groups["remove"].Success;
                    var length = isRemove ? 0 : int.Parse(m.Groups["flength"].Value);
                    if (isRemove)
                    {
                        if (boxes.ContainsKey(key))
                        {
                            var cnt = boxes[key].lenses.RemoveAll(l => l.Label == label);
                            boxes[key].operations.Add(ope);
                        }
                    }
                    else
                    {
                        if (!boxes.ContainsKey(key))
                        {
                            boxes[key] = (new List<Lens> { new Lens(label, length) }, new List<string> { ope });
                        }
                        else
                        {
                            var x = boxes[key].lenses.FirstOrDefault(l => l.Label == label);
                            if (x != default)
                            {
                                x.Length = length;
                            }
                            else
                            {
                                boxes[key].lenses.Add(new Lens(label, length));
                            }
                            boxes[key].operations.Add(ope);
                        }
                    }
                }
            }
            Print(boxes);
            total2 = GetFocusingPower(boxes);

            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 15 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private long GetFocusingPower(Dictionary<int, (List<Lens> lenses, List<string> operations)> boxes)
        {
            long result = 0;
            foreach (var kvp in boxes)
            {
                for (int i = 0; i < kvp.Value.lenses.Count; i++)
                {
                    result += (kvp.Key + 1) * (i + 1) * kvp.Value.lenses[i].Length;
                }
            }
            return result;
        }

        private void Print(Dictionary<int, (List<Lens> lenses, List<string> operations)> boxes)
        {
            foreach (var kvp in boxes.OrderBy(kvp => kvp.Key))
            {
                Console.Write($"BOX {kvp.Key}\t");
                foreach (var lens in kvp.Value.lenses)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"[{lens.Label} {lens.Length}]");
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\t" + string.Join(',', kvp.Value.operations));
                Console.ResetColor();
            }
        }

        private int HASH(string input)
        {
            int result = 0;
            var bytes = System.Text.Encoding.ASCII.GetBytes(input);
            for (int i = 0; i < bytes.Length; i++)
            {
                result += (int)(bytes[i]);
                result *= 17;
                result %= 256;
            }
            return result;
        }
    }
}

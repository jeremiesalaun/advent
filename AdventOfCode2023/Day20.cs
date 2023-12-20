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
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AdventOfCode2023
{
    internal class Day20
    {
        private enum pulse
        {
            low, high
        }
        private enum moduleType
        {
            none, flipflop, conjuction
        }

        private class module
        {
            public string name { get; set; }
            public moduleType type { get; set; }
            public List<string> inputmodules { get; set; } = [];
            public List<string> destmodules { get; set; } = [];

            public bool? flipflopstate { get; set; }
            private Dictionary<string, pulse> lastReceived { get; set; }

            public module(string name, moduleType type)
            {
                this.name = name;
                this.type = type;
            }

            public void Init()
            {
                if (type == moduleType.flipflop)
                {
                    this.flipflopstate = false;
                }
                if (type == moduleType.conjuction)
                {
                    lastReceived = new Dictionary<string, pulse>();
                    foreach (var m in inputmodules)
                    {
                        lastReceived[m] = pulse.low;
                    }
                }
            }

            public List<(string to, pulse value)> Handle(pulse input, string from)
            {
                switch (type)
                {
                    case moduleType.flipflop:
                        return HandleFlipFlop(input);
                    case moduleType.conjuction:
                        return HandleCon(input, from);
                    default:
                        return send(input);
                }
            }

            private List<(string to, pulse value)> HandleCon(pulse input, string from)
            {
                lastReceived[from] = input;
                if (lastReceived.All(kvp => kvp.Value == pulse.high))
                {
                    return send(pulse.low);
                }
                else
                {
                    return send(pulse.high);
                }
            }

            private List<(string to, pulse value)> HandleFlipFlop(pulse input)
            {
                if (input == pulse.high) return null;
                this.flipflopstate = !this.flipflopstate.Value;
                var output = this.flipflopstate.Value ? pulse.high : pulse.low;
                return send(output);
            }

            private List<(string to, pulse value)> send(pulse output)
            {
                return destmodules.Select(m => (to: m, value: output)).ToList();
            }

            internal void PrintInputStates(long cnt)
            {
                if (type != moduleType.conjuction) return;
                if (lastReceived.Any(kvp => kvp.Value == pulse.high))
                {
                    Console.Write($"{cnt}\t");
                    foreach (var kvp in lastReceived)
                    {
                        Console.Write($"{kvp.Key}={kvp.Value}\t");
                    }
                    Console.WriteLine();
                }
            }
        }

        private List<module> modules = [];
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 20 !! ##################################\r\n");
            //Read and parse file
            var inputpath = @"Inputs\day20.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    modules.Add(ParseLine(line));
                }
            }
            //Compute input modules
            foreach (var m in modules)
            {
                m.inputmodules.AddRange(modules.Where(x => x.destmodules.Any(n => n == m.name)).Select(m => m.name));
                m.Init();
            }

            long total1 = 0;
            int nbHigh = 0, nbLow = 0;
            for (int i = 0; i < 1000; i++)
            {
                var r = PushButton(i);
                nbHigh += r.nbHigh;
                nbLow += r.nbLow;
            }
            total1 = nbHigh * nbLow;
            Console.WriteLine($"Got {nbLow} lows and {nbHigh} highs");
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;
            //modules.ForEach(m => m.Init());
            //while (total2<100000)
            //{
            //    PushButton(total2);
            //    total2++;
            //}
            //Calculer la fréquence de L modules concentrateurs suivants : VG,NB,VC et LS
            var frequencies = new long[] {3931,3851,3881,3943};
            //Puis faire un LCM de ces 4.
            total2 = LcmHelper.CalculateLCM(frequencies);

            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 20 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private void Print(module m)
        {
            var q = new Queue<module>();
            q.Enqueue(m);
            while (q.Any())
            {
                var a = q.Dequeue();
                Console.WriteLine(a.name);
                foreach (var d in a.destmodules)
                {
                    Console.Write(d + "\t");
                    var b = modules.FirstOrDefault(m => m.name == d);
                    if (b != null)
                    {
                        q.Enqueue(b);
                    }
                }
                Console.WriteLine();
            }
        }

        private (int nbHigh, int nbLow) PushButton(long cnt)
        {
            int nbHigh = 0, nbLow = 0;
            var pulses = new Queue<(string to, string from, pulse value)>();
            pulses.Enqueue(("broadcaster", "button", pulse.low));
            while (pulses.Any())
            {
                var p = pulses.Dequeue();
                if (p.value == pulse.low) nbLow++;
                if (p.value == pulse.high) nbHigh++;
                var m = modules.FirstOrDefault(m => m.name == p.to);
                if (m != null)
                {
                    if (m.name == "lg")
                    {
                        m.PrintInputStates(cnt);
                    }
                    var res = m.Handle(p.value, p.from)?.Select(r => (r.to, from: m.name, r.value)).ToList();
                    res?.ForEach(r => pulses.Enqueue(r));
                }
            }
            return (nbHigh, nbLow);
        }

        private module ParseLine(string? line)
        {
            var re = new Regex(@"(?<type>[%&]?)(?<name>\w+) -> ((?<dest>\w+)(, )?)+");
            moduleType type = moduleType.none;
            var match = re.Match(line);
            if (match.Success)
            {
                if (match.Groups["type"].Success)
                {
                    if (match.Groups["type"].Value == "%")
                    {
                        type = moduleType.flipflop;
                    }
                    else if (match.Groups["type"].Value == "&")
                    {
                        type = moduleType.conjuction;
                    }
                }
                var mod = new module(match.Groups["name"].Value, type);
                foreach (Capture c in match.Groups["dest"].Captures)
                {
                    mod.destmodules.Add(c.Value);
                }
                return mod;
            }
            return null;
        }
    }
}

using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Day24
    {
        private enum operators
        {
            set,
            and,
            or,
            xor
        }

        private class Gate
        {
            public bool Evaluated { get; private set; }
            public string Name { get; set; }
            public operators Operator { get; set; }

            public string pOneName { get; set; }
            public string pTwoName { get; set; }
            public Gate PredecessorOne { get; set; }
            public Gate PredecessorTwo { get; set; }

            private bool? inputOne;
            public bool? InputOne
            {
                get => inputOne;
                set { inputOne = value; }
            }


            private bool? inputTwo;
            public bool? InputTwo
            {
                get => inputTwo;
                set { inputTwo = value;}
            }

            public bool? Output { get; set; }

            public void Eval()
            {
                if(Evaluated) return;
                if (PredecessorOne != null) { InputOne = PredecessorOne.Output; }
                if (PredecessorTwo != null) { InputTwo = PredecessorTwo.Output; }
                if (Operator == operators.set && InputOne.HasValue)
                {
                    Output = InputOne.Value;
                    Evaluated = true;
                }
                else if (InputOne.HasValue && InputTwo.HasValue)
                {
                    switch (Operator)
                    {
                        case operators.and:
                            Output = InputOne.Value & InputTwo.Value; break;
                        case operators.or:
                            Output = InputOne.Value | InputTwo.Value; break;
                        case operators.xor:
                            Output = InputOne.Value ^ InputTwo.Value; break;
                    }
                    Evaluated = true;
                }
            }
        }

        private Dictionary<String, Gate> gates = new Dictionary<string, Gate>();
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 24 !! ##################################\r\n");
            long total1 = 0;
            string total2 = "";
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day24.txt";
            var path2 = $@"{(TEST ? "Samples" : "Inputs")}\day24-2.txt";

            foreach (var l in File.ReadAllLines(path))
            {
                var s = l.Split(':').Select(s => s.Trim()).ToArray();
                var g = new Gate() { Name = s[0], Operator = operators.set, InputOne = s[1] == "1" };
                gates.Add(g.Name, g);
            }

            var regate = new Regex(@"(?<in1>\w+) (?<op>\w+) (?<in2>\w+) -> (?<name>\w+)");
            foreach (var l in File.ReadAllLines(path2))
            {
                var m = regate.Match(l);
                var g = new Gate()
                {
                    Name = m.Groups["name"].Value,
                    Operator = GetOperator(m.Groups["op"].Value),
                    pOneName = m.Groups["in1"].Value,
                    pTwoName = m.Groups["in2"].Value,
                };
                gates.Add(g.Name, g);
            }
            foreach (var g in gates.Values)
            {
                if (g.pOneName != null) g.PredecessorOne = gates[g.pOneName];
                if (g.pTwoName != null) g.PredecessorTwo = gates[g.pTwoName];
            }


            gates.Values.Where(g => g.Operator == operators.set && !g.Evaluated).ToList().ForEach(g => g.Eval());
            var nonEvaluated = gates.Values.Where(g => g.Operator != operators.set && !g.Evaluated).ToList();
            while (nonEvaluated.Count > 0)
            {
                nonEvaluated.ForEach(g => g.Eval());
                nonEvaluated = gates.Values.Where(g => g.Operator != operators.set && !g.Evaluated).ToList();
            }

            var result = gates.Values.Where(g=>g.Name.StartsWith('z')).OrderByDescending(g=>g.Name).ToList();
            var sb = new StringBuilder();
            foreach(var g in result)
            {
                sb.Append(g.Output.GetValueOrDefault() ? "1" : "0");
                Console.WriteLine($"{g.Name}={(g.Output.GetValueOrDefault()?"1":"0")}");
            }
            total1 = Convert.ToInt64(sb.ToString(),2);

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 24 ***********************************");
            Thread.Sleep(1000);
        }

        private operators GetOperator(string value)
        {
            switch (value)
            {
                case "AND": return operators.and;
                case "OR": return operators.or;
                case "XOR": return operators.xor;
            }
            return operators.set;
        }
    }
}


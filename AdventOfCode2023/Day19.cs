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
using System.Net.WebSockets;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AdventOfCode2023
{
    internal class Day19
    {

        private class workflow
        {
            public string name { get; set; }
            public List<rule> rules { get; set; } = [];

            public string Process(part p)
            {
                foreach (rule r in rules)
                {
                    if (r.Test(p))
                    {
                        return r.next;
                    }
                }
                Console.WriteLine("Probleme");
                return "R";
            }

            internal partRange? AdjustRange(partRange pr,int ruleIndex)
            {
                //All the rules before the target must fail
                var result = pr.Clone();
                for(int i = 0; i < ruleIndex; i++)
                {
                    rules[i].ApplyRule(result, true);
                    if (!result.IsValid()) break;
                }
                //Target rule must succeed.
                rules[ruleIndex].ApplyRule(result, false);
                if (result.IsValid())
                {
                    result.currentWorkflow = this.name;
                    return result;
                }
                return null;
            }
        }

        private class rule
        {
            public string next { get; set; }
            public string prop { get; set; }
            public string op { get; set; }
            public int value { get; set; }

            public bool Test(part p)
            {
                if (prop == null) return true;
                var testValue = 0;
                switch (prop)
                {
                    case "x": testValue = p.x; break;
                    case "m": testValue = p.m; break;
                    case "a": testValue = p.a; break;
                    case "s": testValue = p.s; break;
                }
                switch (op)
                {
                    case "<": return testValue < value;
                    case ">": return testValue > value;
                }
                Console.WriteLine("Probleme");
                return false;
            }

            internal void ApplyRule(partRange pr, bool mustFail)
            {
                if (prop == null)//Catch-all rule : always good
                {
                    if (mustFail) pr.maxx = 0; //Make the rule invalid
                    return;
                }
                int min = 0, max = 4000;
                switch (op)
                {
                    case "<": if (mustFail) { min = value; } else { max = value - 1; } break;
                    case ">": if (mustFail) { max = value; } else { min = value + 1; } break;
                }
                switch (prop)
                {
                    case "x": pr.minx = Math.Max(pr.minx, min); pr.maxx = Math.Min(pr.maxx, max); break;
                    case "m": pr.minm = Math.Max(pr.minm, min); pr.maxm = Math.Min(pr.maxm, max); break;
                    case "a": pr.mina = Math.Max(pr.mina, min); pr.maxa = Math.Min(pr.maxa, max); break;
                    case "s": pr.mins = Math.Max(pr.mins, min); pr.maxs = Math.Min(pr.maxs, max); break;
                }
            }
        }

        private class partRange
        {
            public string currentWorkflow { get; set; }
            public int minx { get; set; } = 1;
            public int minm { get; set; } = 1;
            public int mina { get; set; } = 1;
            public int mins { get; set; } = 1;
            public int maxx { get; set; } = 4000;
            public int maxm { get; set; } = 4000;
            public int maxa { get; set; } = 4000;
            public int maxs { get; set; } = 4000;

            internal partRange Clone()
            {
                return new partRange()
                {
                    currentWorkflow = currentWorkflow,
                    minx = minx,
                    minm = minm,
                    mina = mina,
                    mins = mins,
                    maxx = maxx,
                    maxm = maxm,
                    maxa = maxa,
                    maxs = maxs,
                };
            }

            internal bool IsValid()
            {
                return maxx > minx && maxm > minm && maxa > mina && maxs > mins;
            }

            internal long GetSum()
            {
                return (long)(maxx - minx +1) * (maxm - minm + 1) * (maxa - mina + 1) * (maxs - mins + 1);
            }
        }

        private class part
        {
            public int x { get; set; }
            public int m { get; set; }
            public int a { get; set; }
            public int s { get; set; }

            public int TotalRating { get => x + m + a + s; }
        }


        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 19 !! ##################################\r\n");
            var workflows = new Dictionary<string, workflow>();
            var sortedparts = new Dictionary<string, List<part>>();
            sortedparts.Add("R", new List<part>());
            sortedparts.Add("A", new List<part>());
            //Read and parse file
            var inputpath = @"Inputs\day19.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Length == 0) continue;
                    if (line.StartsWith('{'))
                    {
                        sortedparts["in"].Add(ParsePart(line));
                    }
                    else
                    {
                        workflow w = ParseWorkflow(line);
                        workflows.Add(w.name, w);
                        sortedparts.Add(w.name, new List<part>());
                    }
                }
            }

            //Simplify workflows : 
            //Get all single rule wfs and replace them in all other workflows
            while (workflows.Values.Any(wf => wf.rules.Select(r => r.next).Distinct().Count() == 1))
            {
                foreach (var wf in workflows.Values.Where(wf => wf.rules.Select(r => r.next).Distinct().Count() == 1))
                {
                    var n = wf.rules.First().next;
                    wf.rules.Clear();
                    wf.rules.Add(new rule() { next = n });
                }
                var singleWfs = workflows.Values.Where(wf => wf.rules.Count == 1).ToList();
                foreach (var swf in singleWfs)
                {
                    var n = swf.rules.First().next;
                    foreach (var wf in workflows.Values)
                    {
                        foreach (var r in wf.rules)
                        {
                            if (r.next == swf.name) r.next = n;
                        }
                    }
                    workflows.Remove(swf.name);
                }
            }
            //Eliminate redundant rules (rules before the discard rule that have the same result)
            foreach (var w in workflows.Values)
            {
                var discard = w.rules.Last().next;
                for (var i = w.rules.Count - 2; i >= 0; i--)
                {
                    if (w.rules[i].next != discard) break;
                    w.rules.RemoveAt(i);
                }
            }


            long total1 = 0;
            while (sortedparts.Any(kvp => kvp.Key != "A" && kvp.Key != "R" && kvp.Value.Count > 0))
            {
                foreach (var w in workflows.Values)
                {
                    var partsToSort = sortedparts[w.name];
                    if (partsToSort.Count == 0) continue;
                    for (int i = partsToSort.Count - 1; i >= 0; i--)
                    {
                        var part = partsToSort[i];
                        var dest = w.Process(part);
                        partsToSort.RemoveAt(i);
                        sortedparts[dest].Add(part);
                    }
                }
            }
            var accepted = sortedparts["A"];
            Console.WriteLine($"{accepted.Count} accepted parts");
            total1 = accepted.Sum(p => p.TotalRating);
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;

            //Compute a list of antecedents for all workflows, then process back the rules that goes up to A.
            var antecedents = new Dictionary<string, List<(workflow wf,int ruleIndex)>>();
            antecedents.Add("in", new List<(workflow,int)>());
            foreach (var wf in workflows.Values)
            {
                for(int ruleindex=0;ruleindex<wf.rules.Count;ruleindex++)
                {
                    var next = wf.rules[ruleindex].next;
                    if (antecedents.ContainsKey(next))
                    {
                        antecedents[next].Add((wf,ruleindex));
                    }
                    else
                    {
                        antecedents[next] = new List<(workflow,int)> { (wf,ruleindex) };
                    }
                }
            }
            var results = new List<partRange>();
            var possiblesRanges = new Queue<partRange>();
            possiblesRanges.Enqueue(new partRange() { currentWorkflow = "A" });
            while (possiblesRanges.Count > 0)
            {
                var pr = possiblesRanges.Dequeue();
                if (antecedents.ContainsKey(pr.currentWorkflow))
                {
                    var wfs = antecedents[pr.currentWorkflow];
                    foreach (var w in wfs)
                    {
                        partRange? nextPr = w.wf.AdjustRange(pr,w.ruleIndex);
                        if (nextPr != null)
                        {
                            if (nextPr.currentWorkflow == "in")
                            {
                                results.Add(nextPr);
                            }
                            else
                            {
                                possiblesRanges.Enqueue(nextPr);
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"{results.Count} possible ranges : ");
            foreach (var r in results.OrderBy(r => r.minx).ThenBy(r => r.maxx).ThenBy(r => r.minm).ThenBy(r => r.maxm).ThenBy(r => r.mina).ThenBy(r => r.maxa).ThenBy(r => r.mins).ThenBy(r => r.maxs))
            {
                Console.WriteLine($"{{[{r.minx},{r.maxx}],[{r.minm},{r.maxm}],[{r.mina},{r.maxa}],[{r.mins},{r.maxs}]}}");
                total2 += r.GetSum();
            }


            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 19 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private workflow ParseWorkflow(string line)
        {
            var re = new Regex(@"(?<name>\w+){((?<rule>([xmas]{1}[<>]{1}\d+:){0,1}\w+)[,]{0,1})+}");
            var reRule = new Regex(@"((?<prop>[xmas]{1})(?<op>[<>]{1})(?<value>\d+){1}:){0,1}(?<next>\w+)");
            var match = re.Match(line);
            if (match.Success)
            {
                var w = new workflow();
                w.name = match.Groups["name"].Value;
                foreach (Capture c in match.Groups["rule"].Captures)
                {
                    var mRule = reRule.Match(c.Value);
                    if (mRule.Success)
                    {
                        var r = new rule();
                        r.next = mRule.Groups["next"].Value;
                        if (mRule.Groups["prop"].Success)
                        {
                            r.prop = mRule.Groups["prop"].Value;
                            r.op = mRule.Groups["op"].Value;
                            r.value = int.Parse(mRule.Groups["value"].Value);
                        }
                        w.rules.Add(r);
                    }
                }
                return w;
            }
            return null;
        }

        private part ParsePart(string line)
        {
            var re = new Regex(@"{x=(?<x>\d+),m=(?<m>\d+),a=(?<a>\d+),s=(?<s>\d+)}");
            var match = re.Match(line);
            if (match.Success)
            {
                var part = new part();
                part.x = int.Parse(match.Groups["x"].Value);
                part.m = int.Parse(match.Groups["m"].Value);
                part.a = int.Parse(match.Groups["a"].Value);
                part.s = int.Parse(match.Groups["s"].Value);
                return part;
            }
            return null;
        }
    }
}

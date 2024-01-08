using Rubjerg.Graphviz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data.Common;
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
    internal class Day25
    {
        private class module
        {
            public string name { get; set; }
            public string group { get; set; }
            public List<wire> wiredmodules { get; set; } = [];

            public module(string name)
            {
                this.name = name;
            }
            public override string ToString()
            {
                return name;
            }
        }
        
        private class wire
        {
            public string destname { get; set; }
            public int strength { get; set; }

            public wire(string destname)
            {
                this.destname = destname;
                this.strength = 1;
            }
        }

        private Dictionary<string,module> modules = [];
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 25 !! ##################################\r\n");
            //Read and parse file
            var inputpath = @"Inputs\day25.txt";
            using (var sr = new StreamReader(inputpath, true))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var m = ParseLine(line);
                    modules.Add(m.name,m);
                }
            }
            //Create missing modules
            var distinctModules = modules.SelectMany(kvp => kvp.Value.wiredmodules).Select(w=>w.destname).Distinct().ToList();
            foreach(var n in distinctModules)
            {
                if (!modules.ContainsKey(n))
                {
                    modules.Add(n, new module(n));
                }
            }
            //Create missing relations
            foreach (var module in modules.Values)
            {
                foreach (var w in module.wiredmodules)
                {
                    var n = modules[w.destname];
                    if (!n.wiredmodules.Exists(w => w.destname == module.name))
                    {
                        n.wiredmodules.Add(new wire(module.name));
                    }
                }
            }

            long total1 = 1;

            //CreateDotFile();
            //RenderDotFile();
            var groupA = new List<string>();
            var q = new Queue<string>();
            q.Enqueue("gxv");
            while (q.Any()) 
            {
                var s = q.Dequeue();
                var m = modules[s];
                if (!groupA.Contains(m.name))
                {
                    groupA.Add(m.name);
                }
                foreach(var v in m.wiredmodules)
                {
                    if (!groupA.Contains(v.destname))
                    {
                        q.Enqueue(v.destname);
                    }
                }
            }

            var groupB = new List<string>();
            q.Enqueue("dkh");
            while (q.Any())
            {
                var s = q.Dequeue();
                var m = modules[s];
                if (!groupB.Contains(m.name))
                {
                    groupB.Add(m.name);
                }
                foreach (var v in m.wiredmodules)
                {
                    if (!groupB.Contains(v.destname))
                    {
                        q.Enqueue(v.destname);
                    }
                }
            }
            total1 = groupA.Count * groupB.Count;
            Console.WriteLine($"Final result for 1st star is : {total1}");

            long total2 = 0;

            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 25 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private void RenderDotFile()
        {
            RootGraph root = RootGraph.FromDotFile("out.dot");

            // We can ask Graphviz to compute a layout and render it to svg
            root.ToSvgFile("dot_out.svg");

        }

        private void CreateDotFile()
        {
            RootGraph root = RootGraph.CreateNew(GraphType.Directed, "Some Unique Identifier");
            var nodes = new Dictionary<string, Node>();
            foreach(var module in modules)
            {
                nodes.Add(module.Key, root.GetOrAddNode(module.Key));
            }
            foreach (var module in modules.Values)
            {
                var n1 = nodes[module.name];
                foreach(var w in module.wiredmodules)
                {
                    var n2 = nodes[w.destname];
                    root.GetOrAddEdge(n1, n2);
                }
            }
            root.ToDotFile("out.dot");
        }

        private module ParseLine(string? line)
        {
            var re = new Regex(@"(?<name>\w+):( (?<dest>\w+))+");
            var match = re.Match(line);
            if (match.Success)
            {
                var mod = new module(match.Groups["name"].Value);
                foreach (Capture c in match.Groups["dest"].Captures)
                {
                    mod.wiredmodules.Add(new wire(c.Value));
                }
                return mod;
            }
            return null;
        }
    }
}

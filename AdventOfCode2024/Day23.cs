using System.Data;

namespace AdventOfCode2024
{
    internal class Day23
    {

        private class Node
        {
            public string Name { get; set; }
            public HashSet<Node> Neighbors { get; set; } = new HashSet<Node>();
            public override string ToString()
            {
                return Name;
            }
        }

        private class Triplet
        {
            public Triplet(Node n1, Node n2, Node n3)
            {
                var l = new List<Node>() { n1, n2, n3 };
                l = l.OrderBy(n => n.Name).ToList();
                this.n1 = l[0];
                this.n2 = l[1];
                this.n3 = l[2];
            }

            public Node n1 { get; set; }
            public Node n2 { get; set; }
            public Node n3 { get; set; }

            public override bool Equals(object? obj)
            {
                var t = obj as Triplet;
                if (t == null) return false;
                return this.n1 == t.n1 && this.n2 == t.n2 && this.n3 == t.n3;
            }

            public override string ToString()
            {
                return $"{n1.Name},{n2.Name},{n3.Name}";
            }

            public bool TwoEquals(Triplet t)
            {
                var l1 = new List<Node> { n1, n2, n3 };
                var l2 = new List<Node> { t.n1, t.n2, t.n3 };
                return l1.Intersect(l2).Count() == 2;
            }
        }

        private Dictionary<string, Node> nodes;
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 23 !! ##################################\r\n");
            long total1 = 0;
            string total2 = "";
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day23.txt";

            nodes = new Dictionary<string, Node>();
            foreach (var s in File.ReadLines(path))
            {
                var ns = s.Split('-');
                Node n1;
                Node n2;
                if (nodes.ContainsKey(ns[0]))
                {
                    n1 = nodes[ns[0]];
                }
                else
                {
                    n1 = new Node() { Name = ns[0] };
                    nodes.Add(ns[0], n1);
                }
                if (nodes.ContainsKey(ns[1]))
                {
                    n2 = nodes[ns[1]];
                }
                else
                {
                    n2 = new Node() { Name = ns[1] };
                    nodes.Add(ns[1], n2);
                }
                n1.Neighbors.Add(n2);
                n2.Neighbors.Add(n1);
            }
            var triplets = new List<Triplet>();
            foreach (var n1 in nodes.Values)
            {
                foreach (var n2 in n1.Neighbors)
                {
                    foreach (var n3 in n2.Neighbors)
                    {
                        if (n3.Neighbors.Contains(n1))
                        {
                            var t = new Triplet(n1, n2, n3);
                            if (!triplets.Contains(t))
                            {
                                triplets.Add(t);
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"Found {triplets.Count} triplets");
            //foreach (var t in triplets.OrderBy(t => t.ToString()))
            //{
            //    Console.WriteLine($"{t.n1.Name},{t.n2.Name},{t.n3.Name}");
            //}
            total1 = triplets.Where((Triplet x) => x.n1.Name.StartsWith('t')
                                                || x.n2.Name.StartsWith('t')
                                                || x.n3.Name.StartsWith('t'))
                            .Count();


            int higherCount = 0;
            HashSet<Node> bestNetwork=null;
            foreach (var t in triplets)
            {
                var network = new HashSet<Node>() { t.n1, t.n2, t.n3 };
                foreach (var t2 in triplets.Where(x => x.TwoEquals(t)))
                {
                    if (!network.Contains(t2.n1) && TestNetwork(network, t2.n1)) network.Add(t2.n1);
                    if (!network.Contains(t2.n2) && TestNetwork(network, t2.n2)) network.Add(t2.n2);
                    if (!network.Contains(t2.n3) && TestNetwork(network, t2.n3)) network.Add(t2.n3);                   
                }
                if(network.Count > higherCount)
                {
                    Console.WriteLine($"Found a network with length {network.Count}");
                    bestNetwork = network;
                    higherCount = network.Count;
                }
            }
            if (bestNetwork != null)
            {
                total2 = bestNetwork.Select(n => n.Name).OrderBy(s => s).Aggregate((s1, s2) => s1 + "," + s2);
            }


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 23 ***********************************");
            Thread.Sleep(1000);
        }

        private bool TestNetwork(HashSet<Node> network, Node n1)
        {
            foreach (var node in network)
            {
                if (!node.Neighbors.Contains(n1))
                {
                    return false;
                }
            }
            return true;
        }
    }
}


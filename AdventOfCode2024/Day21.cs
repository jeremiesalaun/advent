using AdventOfCode2024.Helpers;
using System.Drawing;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Channels;

namespace AdventOfCode2024
{
    //Replace 21 by day number
    internal class Day21
    {

        private char[,] nPadMap;
        private char[,] dPadMap;
        private List<string> codes;
        private Dictionary<(Point, Point), string> CacheBest3 = new Dictionary<(Point, Point), string>();
        private Dictionary<(char, char), string> CacheBest5D = new Dictionary<(char, char), string>();

        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 21 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day21.txt";
            codes = File.ReadAllLines(path).ToList();

            InitDPadMap();
            InitNPadMap();


            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            if (!(i == 3 && j == 0) && !(k == 3 && l == 0))
                            {
                                CacheBest3[(new Point(i, j), new Point(k, l))] = FindBest3(new Point(i, j), new Point(k, l));
                            }
                        }
                    }
                }
            }

            foreach (var code in codes)
            {
                var initPos = GetCharPosN('A');
                string res = "";
                foreach (var c in code)
                {
                    var pos = GetCharPosN(c);
                    res += CacheBest3[(initPos, pos)];
                    initPos = pos;
                }

                Console.WriteLine(res);
                total1 += int.Parse(code.Replace("A", "")) * res.Length;
                Console.WriteLine($"{code} : {res.Length}");
            }


            foreach (var c1 in new char[] {'<', '^', 'v', '>','A'})
            {
                foreach (var c2 in new char[] { '<', '^', 'v', '>', 'A' })
                {
                    CacheBest5D[(c1, c2)] = FindBest5D(c1, c2);
                }
            }

            foreach (var code in codes)
            {
                var initPos = GetCharPosN('A');
                string res = "";
                foreach (var c in code)
                {
                    var pos = GetCharPosN(c);
                    res += FindBest25(initPos, pos);
                    initPos = pos;
                }

                Console.WriteLine(res);
                total2 += int.Parse(code.Replace("A", "")) * res.Length;
                Console.WriteLine($"{code} : {res.Length}");
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 21 ***********************************");
            Thread.Sleep(1000);
        }

        private Point NNO = new Point(3, 0);
        private Point DNO = new Point(0, 0);
        private string FindBest3(Point prevPosition, Point nextPosition)
        {
            Console.WriteLine($"Move of {nPadMap.Get(prevPosition)} to {nPadMap.Get(nextPosition)} ({nextPosition.Substract(prevPosition)}) :");
            var outputs = new List<List<char>>();
            var (l1, l2) = Test2(prevPosition, nextPosition, NNO);
            outputs.Add(l1);
            if (l2 != null) outputs.Add(l2);

            var outputs2 = new List<List<char>>();
            foreach (var l in outputs)
            {
                outputs2.AddRange(Recurs(new List<char>(), l, 0));
            }

            var outputs3 = new List<List<char>>();
            foreach (var l in outputs2)
            {
                outputs3.AddRange(Recurs(new List<char>(), l, 0));
            }

            //foreach (var s in outputs3.Select(l => new string(l.ToArray())).Distinct().OrderBy(s => s.Length))
            //{
            //    Console.WriteLine($"\t{new String(s.ToArray())}");
            //}
            return outputs3.OrderBy(s => s.Count).Take(1).Select(l => new string(l.ToArray())).First();
        }


        private string FindBest25(Point prevPosition, Point nextPosition)
        {
            Console.WriteLine($"Move of {nPadMap.Get(prevPosition)} to {nPadMap.Get(nextPosition)} ({nextPosition.Substract(prevPosition)}) :");
            var outputs = new List<List<char>>();
            var (l1, l2) = Test2(prevPosition, nextPosition, DNO);
            outputs.Add(l1);
            if (l2 != null) outputs.Add(l2);

            //Robots 1->4
            var outputs2 = new List<string>();
            var res = new StringBuilder();
            foreach (var l in outputs)
            {
                res.Clear();
                var initPos = 'A';
                for (int i = 0; i < l.Count; i++)
                {
                    res.Append(CacheBest5D[(initPos, l[i])]);
                    initPos = l[i];
                }
                outputs2.Add(res.ToString());
            }
            var bestOutput = outputs2.OrderBy(s => s.Length).Take(1).First();

            for (int j = 0; j < 5; j++)
            {
                res.Clear();
                var initPos = 'A';
                for (int i = 0; i < bestOutput.Length; i++)
                {
                    res.Append(CacheBest5D[(initPos, bestOutput[i])]);
                    initPos = bestOutput[i];
                }
                bestOutput = res.ToString();
            }
            return bestOutput;
        }

        private string FindBest5D(char c1, char c2)
        {
            var prevPosition = GetCharPosD(c1);
            var nextPosition = GetCharPosD(c2);
            Console.WriteLine($"Move of {c1} to {c2} ({nextPosition.Substract(prevPosition)}) :");
            var outputs = new List<List<char>>();
            var (l1, l2) = Test2(prevPosition, nextPosition, DNO);
            outputs.Add(l1);
            if (l2 != null) outputs.Add(l2);

            var outputs2 = new List<List<char>>();
            foreach (var l in outputs)
            {
                outputs2.AddRange(Recurs(new List<char>(), l, 0));
            }

            var outputs3 = new List<List<char>>();
            var min = outputs2.Min(l => l.Count);
            foreach (var l in outputs2.Where(l=>l.Count==min))
            {
                outputs3.AddRange(Recurs(new List<char>(), l, 0));
            }

            var outputs4 = new List<List<char>>();
            min = outputs3.Min(l => l.Count);
            foreach (var l in outputs3.Where(l => l.Count == min))
            {
                outputs4.AddRange(Recurs(new List<char>(), l, 0));
            }

            var outputs5 = new List<List<char>>();
            min = outputs4.Min(l => l.Count);
            foreach (var l in outputs4.Where(l => l.Count == min))
            {
                outputs5.AddRange(Recurs(new List<char>(), l, 0));
            }

            //foreach (var s in outputs3.Select(l => new string(l.ToArray())).Distinct().OrderBy(s => s.Length))
            //{
            //    Console.WriteLine($"\t{new String(s.ToArray())}");
            //}
            return outputs5.OrderBy(s => s.Count).Take(1).Select(l => new string(l.ToArray())).First();
        }

        private static (List<char> l1, List<char>? l2) Test2(Point prevPosition, Point nextPosition, Point no)
        {
            var delta = nextPosition.Substract(prevPosition);
            char dirV = DirToChar(delta.X < 0 ? dirs.north : dirs.south);
            char dirH = DirToChar(delta.Y < 0 ? dirs.west : dirs.east);
            var output = new List<char>();

            //V before H
            for (int i = 0; i < Math.Abs(delta.X); i++) output.Add(dirV);
            for (int i = 0; i < Math.Abs(delta.Y); i++) output.Add(dirH);
            output.Add('A');
            List<char> output2 = null;

            //We cannot try H before V if H move would go to #
            //If movement is on a single dimension then there is not alternate route, no need to try.            
            if (delta.X != 0 && delta.Y != 0 && !(nextPosition.Y == no.Y && prevPosition.X == no.X))
            {
                //H before V
                output2 = new List<char>();
                for (int i = 0; i < Math.Abs(delta.Y); i++) output2.Add(dirH);
                for (int i = 0; i < Math.Abs(delta.X); i++) output2.Add(dirV);
                output2.Add('A');
            }

            return (output, output2);
        }


        private List<List<char>> Recurs(List<char> current, List<char> input, int position)
        {
            if (position == input.Count)
            {
                return new List<List<char>>() { current };
            }
            var from = (position == 0 ? new Point(0, 2) : GetCharPosD(input[position - 1]));
            var to = GetCharPosD(input[position]);
            var res = new List<List<char>>();
            var (l1, l2) = Test2(from, to, DNO);
            if (l2 != null)
            {
                var l = new List<char>(current);
                l.AddRange(l2);
                res.AddRange(Recurs(l, input, position + 1));
            }
            current.AddRange(l1);
            res.AddRange(Recurs(current, input, position + 1));
            return res;
        }

        private void Print(Queue<char> q)
        {
            while (q.Count > 0)
            {
                Console.Write(q.Dequeue());
            }
            Console.WriteLine();
        }

        private void TranscribeN(Queue<char> input, Queue<char> output, Point start)
        {
            Point from = start;
            while (input.Count > 0)
            {
                char c = input.Dequeue();
                Console.Write(c);
                var to = GetCharPosN(c);
                var delta = to.Substract(from);
                //Si on va vers la colonne 0 alors on fait V avant H
                //Si on va utiliser la touche > (delta.Y>0) alors on fait V avant H
                bool VbeforeH = (to.X == 3 || from.X == 3 || delta.Y > 0);
                char dirV = DirToChar(delta.X < 0 ? dirs.north : dirs.south);
                char dirH = DirToChar(delta.Y < 0 ? dirs.west : dirs.east);
                if (VbeforeH) for (int i = 0; i < Math.Abs(delta.X); i++) output.Enqueue(dirV);
                for (int i = 0; i < Math.Abs(delta.Y); i++) output.Enqueue(dirH);
                if (!VbeforeH) for (int i = 0; i < Math.Abs(delta.X); i++) output.Enqueue(dirV);
                output.Enqueue('A');
                from = to;
            }
            Console.WriteLine();
        }

        private void TranscribeD(Queue<char> input, Queue<char> output, Point start)
        {
            Point from = start;
            while (input.Count > 0)
            {
                char c = input.Dequeue();
                Console.Write(c);
                var to = GetCharPosD(c);
                var delta = to.Substract(from);
                //Si on va vers la colonne 0 alors on fait V avant H
                //Si on va utiliser la touche > (delta.Y>0) alors on fait V avant H
                bool VbeforeH = (to.Y == 0 || from.Y == 0 || delta.Y > 0);
                char dirV = DirToChar(delta.X < 0 ? dirs.north : dirs.south);
                char dirH = DirToChar(delta.Y < 0 ? dirs.west : dirs.east);
                if (VbeforeH) for (int i = 0; i < Math.Abs(delta.X); i++) output.Enqueue(dirV);
                for (int i = 0; i < Math.Abs(delta.Y); i++) output.Enqueue(dirH);
                if (!VbeforeH) for (int i = 0; i < Math.Abs(delta.X); i++) output.Enqueue(dirV);
                output.Enqueue('A');
                from = to;
            }
            Console.WriteLine();
        }

        private Dictionary<char,Point> dCharCache =null;
        private Point GetCharPosD(char input)
        {
            if (dCharCache == null)
            {
                dCharCache = new Dictionary<char,Point>();
                dCharCache['<'] = new Point(1, 0);
                dCharCache['^'] = new Point(0, 1);
                dCharCache['v'] = new Point(1, 1);
                dCharCache['>'] = new Point(1, 2);
                dCharCache['A'] = new Point(0, 2);
            }
            return dCharCache[input];
        }

        private Point GetCharPosN(char input)
        {
            return nPadMap.AsPointEnumerable().First(p => nPadMap.Get(p) == input);
        }
        private void InitDPadMap()
        {
            dPadMap = new char[2, 3];
            dPadMap[0, 0] = '#'; dPadMap[0, 1] = '^'; dPadMap[0, 2] = 'A';
            dPadMap[1, 0] = '<'; dPadMap[1, 1] = 'v'; dPadMap[1, 2] = '>';
        }

        private void InitNPadMap()
        {
            nPadMap = new char[4, 3];
            nPadMap[0, 0] = '7'; nPadMap[0, 1] = '8'; nPadMap[0, 2] = '9';
            nPadMap[1, 0] = '4'; nPadMap[1, 1] = '5'; nPadMap[1, 2] = '6';
            nPadMap[2, 0] = '1'; nPadMap[2, 1] = '2'; nPadMap[2, 2] = '3';
            nPadMap[3, 0] = '#'; nPadMap[3, 1] = '0'; nPadMap[3, 2] = 'A';
        }

        private static char DirToChar(dirs d)
        {
            switch (d)
            {
                case dirs.none: return 'A';
                case dirs.north: return '^';
                case dirs.south: return 'v';
                case dirs.east: return '>';
                case dirs.west: return '<';
            }
            return 'X';
        }
    }
}


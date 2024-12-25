using AdventOfCode2024.Helpers;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace AdventOfCode2024
{
    internal class Day21
    {

        private Point NNO = new Point(3, 0);
        private Point DNO = new Point(0, 0);
        private List<string> codes;
        private Dictionary<(string seq, int depth), long> dicMinLengthCache = new Dictionary<(string seq, int depth), long>();
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 21 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day21.txt";
            codes = File.ReadAllLines(path).ToList();

            total1 = Compute(2);
            total2 = Compute(25);
            
            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 21 ***********************************");
            Thread.Sleep(1000);
        }

        private long Compute(int depth)
        {
            dicMinLengthCache.Clear();
            var res = 0L;
            foreach (var code in codes)
            {
                var prevChar = 'A';
                var minLength = 0L;
                foreach (var c in code)
                {
                    Console.Write(c);
                    var (l1, l2) = FindSeqN(prevChar, c);
                    var x = FindMinLength(l1, depth);
                    if (l2 != null)
                    {
                        var y = FindMinLength(l2, depth);
                        if (y < x) x = y;
                    }
                    minLength += x;
                    prevChar = c;
                }
                Console.WriteLine($" : {minLength}");
                res += long.Parse(code.Replace("A", "")) * minLength;
            }

            return res;
        }

        private long FindMinLength(List<char> list,int depth)
        {
            var seq = new string(list.ToArray());
            if (dicMinLengthCache.ContainsKey((seq, depth)))
            {
                return dicMinLengthCache[(seq, depth)];
            }
            //Trouver les possibilités pour la séquence à ce niveau.
            var subSequences = Transcribe(new List<char>(), list, 0);
            //Si on est au niveau 1, renvoyer la plus courte des 2 séquences
            if (depth == 1)
            {
                var d = subSequences.Min(l => l.Count);
                dicMinLengthCache[(seq, depth)] = d;
                return d;
            }
            else
            {
                //Si on est à  un niveau supérieur, relancer le calcul sur chacune des séquences au niveau suivant.
                var minD = long.MaxValue;
                foreach(var l in subSequences)
                {
                    var m = new List<char>();
                    var d = 0L;
                    foreach(var c in l)
                    {
                        m.Add(c);
                        if (c == 'A')
                        {
                            d += FindMinLength(m, depth - 1);
                            m.Clear();
                        }
                    }
                    minD = Math.Min(minD, d);
                }
                dicMinLengthCache[(seq, depth)] = minD;
                return minD;
            }
        }

        private (List<char> l1, List<char>? l2) FindSeqN(char prevChar, char c)
        {
            var prevPosition = GetCharPosN(prevChar);
            var nextPosition = GetCharPosN(c);
            return GetPossibleSequences(prevPosition, nextPosition, NNO);
        }


        private static (List<char> l1, List<char>? l2) GetPossibleSequences(Point prevPosition, Point nextPosition, Point no)
        {
            var delta = nextPosition.Substract(prevPosition);
            char dirV = DirToChar(delta.X < 0 ? dirs.north : dirs.south);
            char dirH = DirToChar(delta.Y < 0 ? dirs.west : dirs.east);
            List<char> output = null;
            if(!(nextPosition.X == no.X && prevPosition.Y == no.Y)) {
                //V before H
                output = new List<char>();
                for (int i = 0; i < Math.Abs(delta.X); i++) output.Add(dirV);
                for (int i = 0; i < Math.Abs(delta.Y); i++) output.Add(dirH);
                output.Add('A');
            }

            //We cannot try H before V if H move would go to #
            //If movement is on a single dimension then there is not alternate route, no need to try.            
            List<char> output2 = null;
            if (delta.X != 0 && delta.Y != 0 && !(nextPosition.Y == no.Y && prevPosition.X == no.X))
            {
                //H before V
                output2 = new List<char>();
                for (int i = 0; i < Math.Abs(delta.Y); i++) output2.Add(dirH);
                for (int i = 0; i < Math.Abs(delta.X); i++) output2.Add(dirV);
                output2.Add('A');
            }
            if (output != null)
            {
                return (output, output2);
            }
            else
            {
                return (output2,null);
            }
        }

        private List<List<char>> Transcribe(List<char> current, List<char> input, int position)
        {
            if (position == input.Count)
            {
                return new List<List<char>>() { current };
            }
            var from = (position == 0 ? new Point(0, 2) : GetCharPosD(input[position - 1]));
            var to = GetCharPosD(input[position]);
            var res = new List<List<char>>();
            var (l1, l2) = GetPossibleSequences(from, to, DNO);
            if (l2 != null)
            {
                var l = new List<char>(current);
                l.AddRange(l2);
                res.AddRange(Transcribe(l, input, position + 1));
            }
            current.AddRange(l1);
            res.AddRange(Transcribe(current, input, position + 1));
            return res;
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

        private Dictionary<char, Point> dCharNCache = null;
        private Point GetCharPosN(char input)
        {
            if (dCharNCache == null)
            {
                dCharNCache = new Dictionary<char, Point>();
                dCharNCache['7']=new Point(0, 0); 
                dCharNCache['8']=new Point(0, 1); 
                dCharNCache['9']=new Point(0, 2);
                dCharNCache['4']=new Point(1, 0); 
                dCharNCache['5']=new Point(1, 1); 
                dCharNCache['6']=new Point(1, 2);
                dCharNCache['1']=new Point(2, 0); 
                dCharNCache['2']=new Point(2, 1); 
                dCharNCache['3']=new Point(2, 2);            
                dCharNCache['0']=new Point(3, 1);
                dCharNCache['A']=new Point(3, 2);
            }
            return dCharNCache[input];
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


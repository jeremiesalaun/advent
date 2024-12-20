using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Day13
    {
        const bool TEST = false;

        private class Machine
        {
            public Point A { get; set; }
            public Point B { get; set; }
            public (long X,long Y) Prize { get; set; }
        }

        private List<Machine> machines = new List<Machine>();

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 13 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day13.txt";
            var lines = File.ReadAllLines(path);
            int i = 0;
            while(i<lines.Length)
            {
                ProcessLines(lines[i], lines[i+1], lines[i+2]);
                i += 4;
            }
            foreach(var m in machines)
            {
                total1 += CalculateTokens(m);
            }
            foreach (var m in machines)
            {
                m.Prize = (m.Prize.X + 10000000000000, m.Prize.Y + 10000000000000);              
                total2 += CalculateTokens(m,false);
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 13 ***********************************");
            Thread.Sleep(1000);
        }

        private long CalculateTokens(Machine m, bool limitResults=true)
        {
            //Console.WriteLine($"Machine: A=({m.A.X},{m.A.Y}) B=({m.B.X},{m.B.Y}), Prize=({m.Prize.X},{m.Prize.Y})");

            //This is just linear algebra with two equations of two unknowns.
            // Prize.X = nbA * A.X + nbB * B.X
            // Prize.Y = nbA * A.Y + nbB * B.Y
            // Working by susbtitution, and taking care that we only take integer results, the calculation is as follow :
            long nbBnum = (m.A.X * m.Prize.Y) - (m.A.Y * m.Prize.X);
            long nbBdenom = (m.A.X * m.B.Y) - (m.A.Y * m.B.X);
            if (nbBnum % nbBdenom != 0) return 0; //Result is not an integer
            long nbB = nbBnum / nbBdenom;
            long nbAnum = m.Prize.X - nbB * m.B.X;
            if (nbAnum % m.A.X != 0) return 0; //Result is not an integer
            long nbA = nbAnum / m.A.X;

            //Check the limitations to 100 pushes for first star => This limit is never actually used !
            if(limitResults && (nbA > 100 || nbB > 100)) return 0;

            //Strangely there is no "minimal number of token" as for each machine there is at most one solution.

            //Console.WriteLine($"\tGOT THE PRIZE with A={nbA} and B={nbB} for a cost of {nbA * 3 + nbB} tokens");
            return nbA * 3 + nbB;
        }

        private Regex reA = new Regex(@"Button A: X\+(?<x>\d+), Y\+(?<y>\d+)");
        private Regex reB = new Regex(@"Button B: X\+(?<x>\d+), Y\+(?<y>\d+)");
        private Regex rePrize = new Regex(@"Prize: X=(?<x>\d+), Y=(?<y>\d+)");
        private void ProcessLines(string lineA, string lineB, string lineP)
        {
            var res = new Machine();
            var mA = reA.Match(lineA);
            res.A = new Point(int.Parse(mA.Groups["x"].Value), int.Parse(mA.Groups["y"].Value));
            var mB = reB.Match(lineB);
            res.B = new Point(int.Parse(mB.Groups["x"].Value), int.Parse(mB.Groups["y"].Value));
            var mP = rePrize.Match(lineP);
            res.Prize = (long.Parse(mP.Groups["x"].Value), long.Parse(mP.Groups["y"].Value));
            machines.Add(res);
        }
    }
}


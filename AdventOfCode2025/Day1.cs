using System.Data;

namespace AdventOfCode2025
{
    internal class Day1
    {

        private class turn
        {
            public int dir = 1;
            public int distance = 0;
            public int move(int pos)
            {
                return (pos + dir * distance) % 100;
            }

            public int move2(int pos)
            {
                return (pos + dir * distance);
            }
        }

        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 1 ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day1.txt";
            var instructions = File.ReadAllLines(path).Select(s => new turn() { dir = (s[0] == 'L' ? -1 : 1), distance = (int.Parse(s.Substring(1))) }).ToList();

            var curpos = 50;
            foreach (var i in instructions)
            {
                curpos = i.move(curpos);
                if (curpos == 0) total1++;
            }

            curpos = 50;
            foreach (var i in instructions)
            {
                //Since we took care to recalculate curpos to the [0-99] interval, we know that curpos/100=0 which simplifies some formulas.
                var newpos = i.move2(curpos);
                int z = 0;
                if (i.dir > 0)
                {
                    z = (newpos / 100);
                    curpos = newpos - z * 100;
                }
                else if (newpos <= 0)
                {
                    z = (curpos == 0 ? 0 : 1) - (newpos / 100);
                    //The trick is to recalculate correctly the new position in negatives : 0 -> 99 -> 98, so a simple a modulo will not do since (-1%100=-1 and not 99)
                    //curpos = (newpos % 100) + (newpos % 100 < 0 ? 100 : 0);
                    curpos = (newpos + (z+1) * 100)%100;
                }
                else
                {
                    curpos = newpos;
                }
                //Console.WriteLine($"{curpos} + {i.dir * i.distance} -> {newpos}({p}) : {z}");
                total2 += z;
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 1 ***********************************");
            Thread.Sleep(1000);
        }

    }
}


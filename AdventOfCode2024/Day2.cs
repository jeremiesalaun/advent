namespace AdventOfCode2024
{
    internal class Day2
    {
        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 2 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day2.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var report = line.Split(' ').Select(s => int.Parse(s)).ToList();
                    var (safe,level) = isSafe(report);
                    if (safe)
                    {
                        total1 += 1;
                        total2 += 1;
                    }
                    else
                    {
                        if (level == 2) //Cas spécial où le plantage a été détecté en 2 alors que le problème vient de l'élément 0. Exemple 50 51 50 48 46
                        {
                            var report0 = report.Skip(1).ToList();
                            (safe, _) = isSafe(report0);
                            if (safe)
                            {
                                total2 += 1;
                                continue;
                            }
                        }
                        //Dans le cas général, une erreur au niveau N peut être corrigée soit en supprimant l'élément N-1, soit en supprimant l'élément N.
                        var report1 = report.ToList();
                        report1.RemoveAt(level-1);
                        (safe, _) = isSafe(report1);
                        if (safe)
                        {
                            total2 += 1;
                            continue;
                        }
                        var report2 = report.ToList();
                        report2.RemoveAt(level);
                        (safe, _) = isSafe(report2);
                        if (safe)
                        {
                            total2 += 1;
                        }
                    }
                }
            }

            //Print out total result
            Console.WriteLine($"Final result for 1st star is : {total1}");
            Console.WriteLine($"Final result for 2nd star is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 2 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private static (bool safe,int level) isSafe(List<int> report)
        {
            int delta = 0; //Pour stocker la direction (positive ou négative) de la suite.
            for (int i = 1; i < report.Count; i++)
            {
                var d = report[i] - report[i - 1];
                if (delta == 0) delta = d; //Initialisation de la direction
                if (d == 0 || Math.Abs(d) > 3 || delta * d < 0)
                {
                    //Console.WriteLine($"{string.Join(",",report)} is UNSAFE at level {i} (d={d},delta={delta})");
                    return (false,i);
                }
            }
            //Console.WriteLine($"{string.Join(",", report)} is SAFE");
            return (true,-1);
        }
    }
}

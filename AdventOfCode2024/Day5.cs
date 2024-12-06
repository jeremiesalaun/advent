using System.Data;

namespace AdventOfCode2024
{
    internal class Day5
    {
        private class Rule
        {
            public int Before { get; set; }
            public int After { get; set; }

            public bool IsApplicable(List<int> pages)
            {
                return pages.Contains(Before) && pages.Contains(After);
            }

            public bool IsVerified(List<int> pages)
            {
                return pages.IndexOf(Before) < pages.IndexOf(After);
            }

            public void FixPages(List<int> pages)
            {
                pages.RemoveAt(pages.IndexOf(After));
                pages.Insert(pages.IndexOf(Before)+1, After);
                //Console.WriteLine($"Fixing rule {Before}|{After} => {string.Join(',',pages)}");
            }
        }

        private List<Rule> rules = new List<Rule>();
        private List<List<int>> updates = new List<List<int>>();
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 5 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = @"Inputs\day5.txt";
            using (var sr = new StreamReader(path, true))
            {
                bool rulesAreDone = false;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Length == 0)
                    {
                        rulesAreDone = true;
                    }
                    else if(!rulesAreDone)
                    {
                        var r = line.Split('|');
                        rules.Add(new Rule() { Before = int.Parse(r[0]), After = int.Parse(r[1]) });
                    }
                    else
                    {
                        updates.Add(line.Split(',').Select(s => int.Parse(s)).ToList());
                    }
                }
            }
            
            var incorrectUpdates = new List<(List<int> pages,Rule failedRule)>();
            foreach(var pages in updates)
            {
                bool invalid = false;
                foreach(var rule in rules)
                {
                    if (rule.IsApplicable(pages))
                    {
                        if (!rule.IsVerified(pages))
                        {
                            invalid= true;
                            incorrectUpdates.Add((pages,rule));
                            break;
                        }
                    }
                }
                if (!invalid)
                {
                    total1 += pages[pages.Count / 2];
                }
            }
            Console.WriteLine($"There are {incorrectUpdates.Count} updates to fix");
            int totalFixCount = 0;
            foreach(var i in incorrectUpdates)
            {
                var fixCount = 0;
                //Try to optimize the number of check and fixes by sorting the rules so that their fixes are not contradictory.
                //Put all the rules that have a given number as AFTER before all the rules that have the same number as BEFORE
                //e.g. : A|C,B|C,A|B => A|C,A|B,B|C
                var applicableRules = SortRules(rules.Where(r => r.IsApplicable(i.pages)).ToList());
                //Console.WriteLine($"{string.Join(',',i.pages)} is incorrect, {applicableRules.Count} rules are applicable");
                i.failedRule.FixPages(i.pages);
                fixCount++;
                var allRulesOk = false;
                while (!allRulesOk)
                {
                    bool invalid = false;
                    foreach (var rule in applicableRules)
                    {
                        if (!rule.IsVerified(i.pages))
                        {
                            invalid = true;
                            rule.FixPages(i.pages);
                            fixCount++;
                        }
                    }
                    allRulesOk = !invalid;
                }
                //Console.WriteLine($"Fixed after {fixCount} fixes");
                totalFixCount += fixCount;
                total2 += i.pages[i.pages.Count / 2];
            }
            Console.WriteLine($"did {totalFixCount} fixes");
            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 5 ***********************************");
            Thread.Sleep(1000);
        }

        private List<Rule> SortRules(List<Rule> rules)
        {
            //Console.WriteLine($"Unsorted rules : {string.Join(',',rules.Select(r=>$"{r.Before}|{r.After}"))} ");
            var res = new List<Rule>();
            foreach (var rule in rules)
            {
                var i = res.FindIndex(r => r.Before == rule.After);
                if (i < 0)
                {
                    res.Add(rule);
                }
                else
                {
                    res.Insert(i, rule);
                }
            }
            //Console.WriteLine($"Sorted rules : {string.Join(',', res.Select(r => $"{r.Before}|{r.After}"))} ");
            return res;
        }
    }
}


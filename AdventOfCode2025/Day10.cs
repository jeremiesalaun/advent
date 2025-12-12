using AdventOfCode2025.Helpers;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2025
{
    //Replace 10 by day number
    internal partial class Day10
    {

        const bool TEST = false;

        private class OopsException : Exception
        {
            public OopsException():base()
            {

            }

            public OopsException(string message) : base(message)
            {

            }
        }

        private class State
        {
            public bool[] Values { get; set; }
            public int Length { get { return Values.Length; } }
            public override string ToString()
            {
                return string.Join("", Values.Select(b => b ? "#" : "."));
            }
            public void Init(int count)
            {
                Values = new bool[count];
            }

            public void Load(string input)
            {
                Values = new bool[input.Length];
                for (int i = 0; i < input.Length; i++)
                {
                    Values[i] = (input[i] == '#');
                }
            }

            public void LoadFromInt(int count, int value)
            {
                Values = new bool[count];
                var s = value.ToString($"b{count}");
                for (int i = 0; i < count; i++)
                {
                    Values[i] = (s[i] == '1');
                }
            }

            public override bool Equals(object? obj)
            {
                var s = obj as State;
                if (s == null) return false;
                //Compare current state with target
                for (int i = 0; i < s.Values.Length; i++)
                {
                    if (Values[i] != s.Values[i]) return false;
                }
                return true;
            }

            internal State PushButton(Button button)
            {
                var result = new State();
                result.Init(Values.Length);
                Values.CopyTo(result.Values, 0);
                foreach (var i in button.Switches)
                {
                    result.Values[i] = !result.Values[i];
                }
                return result;
            }
        }

        private partial class Machine
        {
            private int lightCount;
            public State CurrentState { get; set; }
            public State TargetState { get; set; }

            public List<Button> Buttons { get; set; }

            public int[] Joltage { get; set; }

            public static Machine? Parse(string input)
            {
                var result = new Machine();
                var m = ParsingRegex().Match(input);
                if (!m.Success) return null;
                //Parse target state
                result.lightCount = m.Groups["target"].Value.Length;
                result.CurrentState = new State();
                result.CurrentState.Init(result.lightCount);
                result.TargetState = new State();
                result.TargetState.Load(m.Groups["target"].Value);
                //Parse buttons
                result.Buttons = new List<Button>();
                foreach (Capture c in m.Groups["button"].Captures)
                {
                    result.Buttons.Add(Button.Parse(c.Value));
                }
                //Parse Joltage
                result.Joltage = m.Groups["joltage"].Value.Split(',').Select(int.Parse).ToArray();

                return result;
            }

            private Dictionary<string, Dictionary<string, Button>> matrix;
            public void ComputeStateMatrix()
            {
                matrix = new Dictionary<string, Dictionary<string, Button>>();
                var states = new List<State>();
                for (int i = 0; i < Math.Pow(2, lightCount); i++)
                {
                    var state = new State();
                    state.LoadFromInt(lightCount, i);
                    states.Add(state);
                }
                foreach (var stateFrom in states)
                {
                    var sF = stateFrom.ToString();
                    foreach (var button in Buttons)
                    {
                        var stateTo = stateFrom.PushButton(button);
                        var sT = stateTo.ToString();
                        Dictionary<string, Button> x;
                        if (!matrix.ContainsKey(sF))
                        {
                            x = new Dictionary<string, Button>();
                            matrix[sF] = x;
                        }
                        else
                        {
                            x = matrix[sF];
                        }
                        if (!x.ContainsKey(sT))
                        {
                            x[sT] = button;
                        }
                    }
                }
            }


            private int? currentShortest;
            Queue<(string from, string to, int currentCost, HashSet<string> alreadyVisited)> computeQueue;
            internal int? ComputeShortest()
            {
                _cache = new Dictionary<(string from, string to), int>();
                currentShortest = null;
                computeQueue = new Queue<(string from, string to, int currentCost, HashSet<string> alreadyVisited)>();
                computeQueue.Enqueue((this.CurrentState.ToString(), this.TargetState.ToString(), 0, new HashSet<string>()));
                while (computeQueue.Any())
                {
                    var item = computeQueue.Dequeue();
                    var r = ComputeShortest(item.from, item.to, item.currentCost, item.alreadyVisited);
                    if (r.HasValue)
                    {
                        currentShortest = Min(currentShortest, r.Value);
                    }
                    if (currentShortest == 1) break;
                }
                return currentShortest;
            }

            private Dictionary<(string from, string to), int> _cache;
            private int? ComputeShortest(string from, string to, int currentCost, HashSet<string> alreadyVisited)
            {
                if (alreadyVisited.Contains(from)) return null;
                alreadyVisited.Add(from);
                if (currentCost >= currentShortest - 1) return null;
                //Console.WriteLine($"searching on [{from}]-[{to}] at level {currentCost}");
                if (_cache.ContainsKey((from, to)))
                {
                    //Console.WriteLine($"-> Found a path using cache in {currentCost + _cache[(from, to)]} with path [{string.Join("]-[", alreadyVisited)}] + cache for [{from}]-[{to}]");
                    return currentCost + _cache[(from, to)];
                }
                var sources = matrix[from];
                int? currentMinCost = null;
                foreach (var source in sources.Keys)
                {
                    UpdateCache(from, source, 1);
                    if (source == to)
                    {
                        currentMinCost = currentCost + 1;
                        //Console.WriteLine($"-> Found a path in {currentMinCost} with path [{string.Join("]-[", alreadyVisited)}]-[{to}]");
                        break;
                    }
                    else
                    {
                        var av = new HashSet<string>(alreadyVisited);
                        computeQueue.Enqueue((source, to, currentCost + 1, av));
                    }
                }
                if (currentMinCost.HasValue) UpdateCache(from, to, currentMinCost.Value);
                //Console.WriteLine($"-> response at level {currentCost} is {currentMinCost}");
                return currentMinCost;
            }

            private static int? Min(int? a, int? b)
            {
                if (b == null) return a;
                if (a == null) return b;
                return Math.Min(a.Value, b.Value);
            }

            private void UpdateCache(string from, string to, int v)
            {
                if (_cache.ContainsKey((from, to)) && _cache[(from, to)] <= v)
                {
                    return;
                }
                _cache[(from, to)] = v;
            }

            [GeneratedRegex(@"\[(?<target>[\.#]+)\] (?<button>\([\d,]+\) )+\{(?<joltage>[\d,]+)\}")]
            private static partial Regex ParsingRegex();

            public override string ToString()
            {
                return $"Target:[{this.TargetState}], Buttons:{String.Join(',', this.Buttons)}, Joltage:{{{String.Join(',', this.Joltage)}}}";
            }


            private (int[,] coefs, int[] output, int[] hits)? savedState;
            private HashSet<(int switchId, int hitCount)> assumptionsMade;

            internal long? ResolveJoltage()
            {
                var buttons = this.Buttons; //.OrderBy(b => b.Switches.Length).ToList();
                var hits = new int[buttons.Count];
                var output = new int[lightCount];
                this.Joltage.CopyTo(output);
                //Prepare equation matrix
                var coefs = new int[buttons.Count, lightCount];
                for (int j = 0; j < buttons.Count; j++)
                {
                    foreach (var k in buttons[j].Switches)
                    {
                        coefs[j, k] = 1;
                    }
                }
                Console.WriteLine("Initial matrix:");
                PrintState(hits, output, coefs);

                var substitutes = new List<(int[] switches, int output)>();
                assumptionsMade = new HashSet<(int switchId, int hitCount)>();
                while (true)
                {
                    try
                    {
                        substitutes.Clear();
                        while (true)
                        {
                            //Find the light with the lowest count of buttons;
                            int? simplestLight = null;
                            int[] simplestSwitches = null;
                            for (int i = 0; i < coefs.GetLength(1); i++)
                            {
                                var x = ExtractColumn(coefs, i);
                                if (x.Sum() != 0 && (simplestSwitches == null || simplestSwitches.Sum() > x.Sum()))
                                {
                                    simplestLight = i;
                                    simplestSwitches = x;
                                }
                            }
                            if (!simplestLight.HasValue) break;
                            //If there is only one, then we have an answer and we can simplify other columns and iterate
                            if (simplestSwitches.Sum() == 1)
                            {
                                var x = simplestSwitches.IndexOf(1);
                                ApplyHit(hits, output, coefs, x, output[simplestLight.Value]);
                            }
                            //Else, we introduce a combined variable and iterate
                            else
                            {
                                var substitute = (switches: simplestSwitches, output: output[simplestLight.Value]); //[0,0,0,1,1] = 125
                                substitutes.Add(substitute);
                                Console.WriteLine($"Creating substitute [{string.Join(",", substitute.switches)}] = {substitute.output}");
                                for (int i = 0; i < coefs.GetLength(1); i++)
                                {
                                    if (containsSwitches(coefs, i, substitute.switches))
                                    {
                                        removeSwitches(coefs, i, substitute.switches);
                                        output[i] -= substitute.output;
                                        if (output[i] < 0)
                                        {
                                            throw new OopsException("Removed too much when doing a substitute");
                                        }
                                    }
                                }
                            }
                            Console.WriteLine("First pass output:");
                            PrintState(hits, output, coefs);
                        }
                        if (!substitutes.Any()) break;
                        //Recreate a coeff matrix with the substitutes
                        coefs = new int[buttons.Count, substitutes.Count];
                        output = new int[substitutes.Count];
                        for (int i = 0; i < substitutes.Count; i++)
                        {
                            var v = substitutes[i].output;
                            for (int j = 0; j < buttons.Count; j++)
                            {                                
                                if (substitutes[i].switches[j] == 1 && hits[j] > 0)
                                {
                                    v -= hits[j];
                                    if(v<0) throw new OopsException("Too much hits while building substitutes matrix");
                                }
                                else if(v>0)
                                {
                                    coefs[j, i] = substitutes[i].switches[j];
                                }                                
                            }
                            output[i] = v;
                        }
                        Console.WriteLine("Substitutes matrix:");
                        PrintState(hits, output, coefs);
                        //Consider all substitutes created. Find the switch that is the most used.
                        //Make a maximum assumption about it
                        savedState = (Copy(coefs), Copy(output), Copy(hits));
                        (int switchId, int hitCount) = FindBestAssumption(coefs, output);
                        assumptionsMade.Add((switchId, hitCount));
                        ApplyHit(hits, output, coefs, switchId, hitCount);
                        ConsistencyCheck(output, coefs);
                        Console.WriteLine("Second pass output:");
                        PrintState(hits, output, coefs);
                        Console.WriteLine("##############################################");
                    }
                    catch (OopsException e)
                    {
                        Console.WriteLine($"OOOPS : {e.Message}");
                        if (savedState != null)
                        {
                            Console.WriteLine("Restoring saved state");
                            coefs = savedState.Value.coefs;
                            output = savedState.Value.output;
                            hits = savedState.Value.hits;
                            PrintState(hits, output, coefs);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                if (output.Sum() > 0)
                {
                    Console.WriteLine("OOPS");
                }
                return hits.Sum();
            }

            private void ConsistencyCheck(int[] output, int[,] coefs)
            {
                //TODO Implement consistency check
                // For each switch, find max value. Then check if we can still reach output by adding max values of switches
                //throw new OopsException("No consistency");
            }

            private static int[] Copy(int[] source)
            {
                var r = new int[source.Length];
                source.CopyTo(r, 0);
                return r;
            }

            private static int[,] Copy(int[,] source)
            {
                var r = new int[source.GetLength(0), source.GetLength(1)];
                for (int i = 0; i < source.GetLength(0); i++)
                {
                    for (int j = 0; j < source.GetLength(1); j++)
                    {
                        r[i,j]=source[i,j];
                    }
                }
                return r;
            }

            private static void PrintState(int[] hits, int[] output, int[,] coefs)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < coefs.GetLength(0); i++)
                {
                    sb.AppendFormat("{0,3}|", hits[i]);
                    for (int j = 0; j < coefs.GetLength(1); j++)
                    {
                        sb.Append(coefs[i, j] == 1 ? "  #|" : "   |");
                    }
                    sb.Append("\r\n");
                }
                sb.AppendLine(new string('-', 4 * (1 + output.Length)));
                sb.Append("    ");
                for (int j = 0; j < output.Length; j++)
                {
                    sb.AppendFormat("{0,3}|", output[j]);
                }
                sb.Append("\r\n");
                Console.WriteLine(sb.ToString());
            }

            private static void ApplyHit(int[] hits, int[] output, int[,] coefs, int switchId, int hitCount)
            {
                hits[switchId] = hitCount;
                for (int i = 0; i < coefs.GetLength(1); i++)
                {
                    if (coefs[switchId, i] == 1)
                    {
                        coefs[switchId, i] = 0;
                        output[i] -= hitCount;
                        if (output[i] < 0)
                        {
                            throw new OopsException("Output got negative when applying a hit");
                        }
                    }
                }
            }

           

            private (int switchId, int hitCount) FindBestAssumption(int[,] coefs, int[] output)
            {
                //Compute total sum of coefs, and output
                int totalOutput = output.Sum();
                int[] reverseOutput = ComputeReverseOutput(output);
                int[] totalCoefs = new int[coefs.GetLength(0)];
                for (int i = 0; i < coefs.GetLength(0); i++)
                {
                    totalCoefs[i] = ExtractRow(coefs, i).Sum();
                }

                //Compute for each switch the max possible
                int[] maxHits = new int[coefs.GetLength(0)];
                for (int i = 0; i < coefs.GetLength(0); i++)
                {
                    int currentMax = int.MaxValue;
                    for (int j = 0; j < coefs.GetLength(1); j++)
                    {
                        if (coefs[i, j] == 1)
                        {
                            //var sum = ExtractColumn(coefs, j).Sum();
                            currentMax = Math.Min(output[j], currentMax);
                        }
                    }
                    maxHits[i] = currentMax;
                }

                for (int i = 0; i < reverseOutput.Length; i++)
                {
                    var switches = ExtractColumn(coefs, reverseOutput[i]);
                    while (IsMatch(totalCoefs, switches))
                    {
                        Substract(totalCoefs, switches);
                        totalOutput -= output[reverseOutput[i]];
                        if (totalOutput < 0)
                        {
                            throw new OopsException("Gone too far while building assumption");
                        }
                    }
                }
                //Hopefully il n'en reste plus qu'un
                var s = totalCoefs.Sum();
                for (int i = 0; i < totalCoefs.Length; i++)
                {
                    if (totalCoefs[i] > 0 && totalCoefs[i] == s)
                    {
                        var v = Math.Min(totalOutput / totalCoefs[i], maxHits[i]);
                        if (assumptionsMade.Contains((i, v))) continue; //It was not a good bet, go to next
                        Console.WriteLine($"My best bet is on {i} with {v} hits");
                        return (switchId: i, hitCount: v);
                    }
                }
                Console.WriteLine($"Oops, did not find any match, fall back on first strategy !");
                return FindBestAssumption2(coefs, output);
            }

            private (int switchId, int hitCount) FindBestAssumption2(int[,] coefs, int[] output)
            {
                int mostUsed = -1;
                int mostUsedCnt = -1;
                int minUsage = int.MaxValue;
                for (int i = 0; i < coefs.GetLength(0); i++)
                {
                    int localMin = int.MaxValue;
                    var c = 0;
                    for (int j = 0; j < coefs.GetLength(1); j++)
                    {
                        if (coefs[i, j] == 1)
                        {
                            c++;
                            localMin = Math.Min(localMin, output[j]);
                        }
                    }
                    if (c > mostUsedCnt && localMin != int.MaxValue && !assumptionsMade.Contains((i, localMin)))
                    {
                        mostUsedCnt = c;
                        mostUsed = i;
                        minUsage = localMin;
                    }
                }
                if (mostUsed >= 0)
                {
                    Console.WriteLine($"My best bet is on {mostUsed} with {minUsage} hits");
                    return (mostUsed, minUsage);
                }
                Console.WriteLine($"Oops, did not find any match, fall back on last chance strategy !");
                return FindBestAssumption3(coefs, output);
            }

            private (int switchId, int hitCount) FindBestAssumption3(int[,] coefs, int[] output)
            {
                //Let's concentrate on the column with min output which has less options
                int minVal = output.Min();
                int col = output.IndexOf(output.Min());
                var switches = ExtractColumn(coefs, col);
                for (int v = minVal; v >= 0; v--) {
                    for (int i = 0; i < switches.Length; i++)
                    {
                        if (switches[i] == 1 && !assumptionsMade.Contains((i, v)))
                        {
                            Console.WriteLine($"My best bet is on {i} with {v} hits");
                            return (i, v);
                        }
                    }
                }
                throw new OopsException("Nope, no solution at all !");
            }

            private static void Substract(int[] source, int[] pattern)
            {
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (pattern[i] == 1) source[i]--;
                }
            }

            private static bool IsMatch(int[] source, int[] pattern)
            {
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (pattern[i] == 1 && source[i] == 0) return false;
                }
                return true;
            }

            private static int[] ComputeReverseOutput(int[] output)
            {
                //I have a list of unordered int, I want the list of their indexes in descending order
                int[] result = new int[output.Length];
                int[] work = new int[output.Length];
                output.CopyTo(work, 0);
                for (int j = 0; j < result.Length; j++)
                {
                    int localMax = 0;
                    int localMaxPosition = 0;
                    for (int i = 0; i < work.Length; i++)
                    {
                        if (work[i] > localMax)
                        {
                            localMax = work[i];
                            localMaxPosition = i;
                        }
                    }
                    result[j] = localMaxPosition;
                    work[localMaxPosition] = 0;
                }
                return result;
            }

            private void removeSwitches(int[,] coefs, int col, int[] switches)
            {
                for (int row = 0; row < switches.Length; row++)
                {
                    if (switches[row] == 1) coefs[row, col] = 0;
                }
            }

            private bool containsSwitches(int[,] coefs, int col, int[] switches)
            {
                for (int row = 0; row < switches.Length; row++)
                {
                    if (switches[row] == 1 && coefs[row, col] == 0) return false;
                }
                return true;
            }

            private static int[] ExtractColumn(int[,] coefs, int col)
            {
                var res = new int[coefs.GetLength(0)];
                for (int row = 0; row < coefs.GetLength(0); row++)
                {
                    res[row] = coefs[row, col];
                }
                return res;
            }

            private static int[] ExtractRow(int[,] coefs, int row)
            {
                var res = new int[coefs.GetLength(1)];
                for (int col = 0; col < coefs.GetLength(1); col++)
                {
                    res[col] = coefs[row, col];
                }
                return res;
            }
        }

        private class Button
        {
            public int[] Switches { get; set; }

            internal static Button Parse(string value)
            {
                var values = value.Trim().TrimStart('(').TrimEnd(')').Split(',').Select(int.Parse).ToArray();
                return new Button() { Switches = values };
            }

            public override string ToString()
            {
                return "(" + string.Join(',', this.Switches) + ")";
            }
        }


        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 10  ##################################\r\n");
            if (TEST) Console.WriteLine("!! Running in TEST mode on Sample data !!");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day10.txt";
            var machines = File.ReadAllLines(path).Select(Machine.Parse).Where(m => m != null).ToList();
            //foreach (var machine in machines)
            //{
            //    machine.ComputeStateMatrix();
            //    var r = machine.ComputeShortest();
            //    //Console.WriteLine($"Shortest for {machine} is {r}");
            //    total1 += r.GetValueOrDefault();
            //}


            foreach (var machine in machines)
            {
                var r = machine.ResolveJoltage();
                Console.WriteLine($"Shortest for {machine} is {r}");
                total2 += r.GetValueOrDefault();
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 10 ***********************************");
            Thread.Sleep(1000);
        }

    }
}


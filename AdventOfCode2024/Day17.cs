using System.Data;

namespace AdventOfCode2024
{
    internal class Day17
    {
        private enum instructions
        {
            adv,
            bxl,
            bst,
            jnz,
            bxc,
            _out,
            bdv,
            cdv
        }

        private UInt64 A_reg;
        private UInt64 B_reg;
        private UInt64 C_reg;
        private List<int> program;
        private List<UInt64> output = new List<UInt64>();
        private bool enableDebug = false;

        const bool TEST = false;
        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 17 !! ##################################\r\n");
            string total1;
            UInt64 total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day17.txt";
            var lines = File.ReadAllLines(path);
            A_reg = UInt64.Parse(lines[0].Substring(12));
            B_reg = UInt64.Parse(lines[1].Substring(12));
            C_reg = UInt64.Parse(lines[2].Substring(12));
            program = lines[4].Substring(9).Split(",").Select(int.Parse).ToList();

            RunProgram();
            total1 = string.Join(",", output);


            var alreadyCalculated = new HashSet<(UInt64,int)>(); 
            var possibleAnswers = new List<UInt64>();
            var valuesToTest = new Queue<(UInt64 start, UInt64 incr, int digits)>();
            valuesToTest.Enqueue((0, 0, 0));
            while (valuesToTest.Any())
            {
                Console.Write($"\r{valuesToTest.Count}");
                var task = valuesToTest.Dequeue();
                var digits = task.digits + 1;
                var start = (task.start + task.incr) * 8;
                for (ulong i = 0; i < 8; i++)
                {
                    if (!alreadyCalculated.Contains((start + i,digits)))
                    {
                        alreadyCalculated.Add((start + i,digits));
                        if (RunProgramAndCheckOutput(start + i, digits))
                        {
                            if(digits == 16)
                            {
                                possibleAnswers.Add(start + i);
                            }
                            else
                            {
                                valuesToTest.Enqueue((start, i, digits));
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"\rFound {possibleAnswers.Count} targets");
            total2 = possibleAnswers.Min();

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 17 ***********************************");
            Thread.Sleep(1000);
        }

        private void DebugProgram(ulong init)
        {
            enableDebug = true;
            A_reg = init;
            Console.WriteLine($"INIT {A_reg:X16}");
            output.Clear();
            RunProgram();
            Console.WriteLine(string.Join(",", program));
            Console.WriteLine(string.Join(",", output) + $" ({output.Count})");
            enableDebug = false;
        }

        private void RunProgram()
        {
            int i = 0;
            while (i < program.Count)
            {
                i = PerformInstruction(program[i], (UInt64)program[i + 1], i);
            }
        }

        private bool RunProgramAndCheckOutput(ulong init, int digits)
        {
            var wantedOutput = program.Skip(program.Count - digits).Select(i=>(UInt64)i).ToList();
            if (wantedOutput.Count == 0) return true;
            output.Clear();
            A_reg = init;
            B_reg = 0;
            C_reg = 0;
            RunProgram();
            return wantedOutput.SequenceEqual(output);
        }

        private int PerformInstruction(int opcode, UInt64 operand, int instructionPointer)
        {
            instructionPointer += 2;
            var op = (instructions)opcode;
            switch (op)
            {
                case instructions.adv: PerformAdv(operand); break;
                case instructions.bxl: PerformBxl(operand); break;
                case instructions.bst: PerformBst(operand); break;
                case instructions.jnz: instructionPointer = PerformJnz(operand, instructionPointer); break;
                case instructions.bxc: PerformBxc(operand); break;
                case instructions._out: PerformOut(operand); break;
                case instructions.bdv: PerformBdv(operand); break;
                case instructions.cdv: PerformCdv(operand); break;
            }
            return instructionPointer;
        }
        private UInt64 GetCombo(UInt64 operand)
        {
            switch (operand)
            {
                case 0:
                case 1:
                case 2:
                case 3: return operand;
                case 4: return A_reg;
                case 5: return B_reg;
                case 6: return C_reg;
            }
            return 0;
        }
        private UInt64 PerformDivision(UInt64 operand)
        {
            var num = A_reg;
            var div = Math.Pow(2, GetCombo(operand));
            return (UInt64)Math.Floor(num / div);
        }

        private void PerformAdv(UInt64 operand)
        {
            A_reg = PerformDivision(operand);
            //Console.WriteLine($"A div {operand} => A={A_reg}");
        }

        private void PerformBdv(UInt64 operand)
        {
            B_reg = PerformDivision(operand);
            //Console.WriteLine($"A div {operand} => B={B_reg}");
        }

        private void PerformCdv(UInt64 operand)
        {
            C_reg = PerformDivision(operand);
            //Console.WriteLine($"A div {operand} => C={C_reg}");
        }


        private void PerformOut(UInt64 operand)
        {
            var val = GetCombo(operand) % 8;
            output.Add(val);
            if (enableDebug) Console.WriteLine("Output : " + string.Join(",", output));
        }

        private void PerformBxl(UInt64 operand)
        {
            var val = B_reg;
            B_reg = val ^ operand;
            //Console.WriteLine($"{val} xor {operand} => B={B_reg}");
        }

        private void PerformBxc(UInt64 operand)
        {
            var val = B_reg;
            B_reg = val ^ C_reg;
            //Console.WriteLine($"{val} xor {C_reg} => B={B_reg}");
        }
        private void PerformBst(UInt64 operand)
        {
            var val = GetCombo(operand);
            B_reg = val % 8;
            //Console.WriteLine($"{val} mod 8 => B={B_reg}");

        }

        private int PerformJnz(UInt64 operand, int instructionPointer)
        {
            if (A_reg != 0)
            {
                if (enableDebug) Console.WriteLine($"\tjmp {A_reg:X16}");
                //Console.WriteLine($"Jumping to {operand}");
                return (int)operand;
            }
            //Console.WriteLine("Not jumping");
            return instructionPointer;
        }
    }
}


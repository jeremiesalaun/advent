using System.Data;

namespace AdventOfCode2024
{
    internal class Day9
    {
        const bool TEST = false;
        private class Slot
        {
            public string Name { get; set; }
            public bool IsEmpty { get; set; }
            public int Length { get; set; }
        }

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 9 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day9.txt";
            var line = File.ReadAllText(path);
            var input = line.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray();
            var diskLayout = new List<string>();
            var diskLayout2 = new List<Slot>();
            var curFileId = 0;
            var curs = 0;
            var isFile = true;
            while (true)
            {
                if (curs >= input.Length) break;
                var c = isFile ? curFileId.ToString() : ".";
                for (var n = 0; n < input[curs]; n++) diskLayout.Add(c);

                diskLayout2.Add(new Slot() { Name = c, IsEmpty = !isFile, Length = input[curs] });

                if (isFile) curFileId++;
                isFile = !isFile;
                curs++;
            }

            Defrag1(diskLayout);

            for (int i = 0; i < diskLayout.Count; i++)
            {
                total1 += i * (diskLayout[i] == "." ? 0 : int.Parse(diskLayout[i]));
            }

            Defrag2(diskLayout2);
            curs = 0;
            for (int i = 0; i < diskLayout2.Count; i++)
            {
                var s = diskLayout2[i];
                var fileId = s.IsEmpty ? 0 : int.Parse(s.Name);
                for (int k = 0; k < s.Length; k++)
                {
                    total2 += curs * fileId;
                    curs++;
                }
            }


            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 9 ***********************************");
            Thread.Sleep(1000);
        }

        private static void Defrag1(List<string> diskLayout)
        {
            for (int i = diskLayout.Count - 1; i >= 0; i--)
            {
                if (diskLayout[i] != ".")
                {
                    int j = diskLayout.IndexOf(".");
                    if (j >= i) break;
                    diskLayout[j] = diskLayout[i];
                    diskLayout[i] = ".";
                }
                if (TEST) Console.WriteLine($"{diskLayout.Aggregate((s1, s2) => s1 + s2)}");
            }
        }

        private static void Defrag2(List<Slot> diskLayout2)
        {
            for (int i = diskLayout2.Count - 1; i >= 0; i--)
            {
                var s = diskLayout2[i];
                if (s.IsEmpty) continue;
                //Find destination location
                var dest = diskLayout2.FindIndex(x => x.IsEmpty && x.Length >= s.Length);
                if (dest == -1 || dest > i) continue;
                //Insert slot in new location
                diskLayout2.Insert(dest, new Slot { Name = s.Name, IsEmpty = false, Length = s.Length });
                
                //Adapt destination location
                var e = diskLayout2[dest + 1];
                e.Length -= s.Length;
                
                //Adapt source slot : either add the empty space to adjacent empty slots or if there is none transform s in an empty slot
                var newIndex = i+1;
                if (newIndex < diskLayout2.Count - 1 && diskLayout2[newIndex + 1].IsEmpty)
                {
                    diskLayout2[newIndex + 1].Length += s.Length;
                    diskLayout2.RemoveAt(newIndex);
                }
                else if (newIndex >= 1 && diskLayout2[newIndex - 1].IsEmpty)
                {
                    diskLayout2[newIndex - 1].Length += s.Length;
                    diskLayout2.RemoveAt(newIndex);
                }
                else
                {
                    s.Name = ".";
                    s.IsEmpty = true;
                }
                if (TEST)
                {
                    foreach (var x in diskLayout2)
                    {
                        Console.Write(new string(x.Name[0], x.Length));
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}



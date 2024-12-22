using AdventOfCode2024.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    //Replace 21 by day number
    internal class Day21
    {

        private char[,] nPadMap;
        private char[,] dPadMap;
        private List<string> codes;
        private Queue<char> digitalQueue = new Queue<char>();
        private Queue<char> dirQueue1 = new Queue<char>();
        private Queue<char> dirQueue2 = new Queue<char>();
        private Queue<char> dirQueue3 = new Queue<char>();

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
            //InitCacheDPad();
            foreach (var code in codes)
            {
                digitalQueue.Clear();
                dirQueue1.Clear();
                dirQueue2.Clear();
                dirQueue3.Clear();
                for (int i = 0; i < code.Length; i++) digitalQueue.Enqueue(code[i]);

                TranscribeN(digitalQueue, dirQueue1, GetCharPosN('A'));
                TranscribeD(dirQueue1, dirQueue2, GetCharPosD('A'));
                TranscribeD(dirQueue2, dirQueue3, GetCharPosD('A'));
                total1 += int.Parse(code.Replace("A", "")) * dirQueue3.Count;
                Console.WriteLine($"{code} : {dirQueue3.Count}");
                Print(dirQueue3);
            }

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 21 ***********************************");
            Thread.Sleep(1000);
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
                bool VbeforeH = to.Y == 0; //Si on va vers la colonne 0 alors on fait V avant H
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
                bool VbeforeH = to.Y == 0; //Si on va vers la colonne 0 alors on fait V avant H
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

        //private void TranscribeD(Queue<char> input, Queue<char> output, char start)
        //{
        //    char from = start;
        //    while (input.Count > 0)
        //    {
        //        char to = input.Dequeue();
        //        Console.Write(to);
        //        if (from!=to){ 
        //            var l = cacheDPad[(from, to)];
        //            l.ForEach(c=>output.Enqueue(c));
        //        }
        //        output.Enqueue('A');
        //        from = to;
        //    }
        //    Console.WriteLine();
        //}

        //private Dictionary<(char from, char to), List<char>> cacheDPad = new Dictionary<(char from, char to), List<char>>();
        //private void InitCacheDPad()
        //{
        //    cacheDPad[('^', 'A')] = ['>'];
        //    cacheDPad[('A', '^')] = ['<'];

        //    cacheDPad[('A', '>')] = ['v'];
        //    cacheDPad[('>', 'A')] = ['^'];

        //    cacheDPad[('^', 'v')] = ['v'];
        //    cacheDPad[('v', '^')] = ['^'];

        //    cacheDPad[('v', '>')] = ['>'];
        //    cacheDPad[('>', 'v')] = ['<'];

        //    cacheDPad[('<', 'v')] = ['>'];
        //    cacheDPad[('v', '<')] = ['<'];

        //    cacheDPad[('^', '>')] = ['v', '>'];
        //    cacheDPad[('>', '^')] = ['<', '^'];

        //    cacheDPad[('^', '<')] = ['v', '<'];
        //    cacheDPad[('<', '^')] = ['>', '^'];

        //    cacheDPad[('<', '>')] = ['>', '>'];
        //    cacheDPad[('>', '<')] = ['<', '<'];

        //    cacheDPad[('A', 'v')] = ['<', 'v'];
        //    cacheDPad[('v', 'A')] = ['>', '^'];

        //    cacheDPad[('A', '<')] = [ '<', 'v', '<'];
        //    cacheDPad[('<', 'A')] = ['>', '>', '^'];
        //}

        private Point GetCharPosD(char input)
        {
            return dPadMap.AsPointEnumerable().First(p => dPadMap.Get(p) == input);
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


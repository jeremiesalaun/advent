using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Namer
    {
        private List<string> names;
        private char[] letters;
        private int current;
        public Namer() {
            names = new List<string>(File.ReadAllLines("names.txt"));
            letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            current = 0;
        }

        public string NextName()
        {
            var r=  names[current];
            current++;
            return r;
        }

        public string NextLetter()
        {
            var i = current;
            string r="";
            if(i/letters.Length > 0)
            {
                r += letters[(i / letters.Length) - 1];
            }
            r += letters[(i % letters.Length)];
            current++;
            return r;
        }

        public void Reset()
        {
            current = 0;
        }
    }
}

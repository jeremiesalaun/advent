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
        private int current;
        public Namer() {
            names = new List<string>(File.ReadAllLines("names.txt"));
            current = 0;
        }

        public string NextName()
        {
            var r=  names[current];
            current++;
            return r;
        }

        public void Reset()
        {
            current = 0;
        }
    }
}

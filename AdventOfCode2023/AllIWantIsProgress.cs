using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class AllIWantIsProgress
    {
        private char[] lyrics;
        private int current = 0;

        public AllIWantIsProgress() {
            var l = File.ReadAllText("AllIWant.txt");
            lyrics = l.ToCharArray();
            current = 0;
        }

        public void Progress(int value)
        {
            var next = current + value;
            if(next > lyrics.Length)
            {
                current = 0;
                next = 1;
            }
            for(int i=current;i<next;i++)
            {
                Console.Write(lyrics[i]);
            }
            current = next;
        }

    }
}

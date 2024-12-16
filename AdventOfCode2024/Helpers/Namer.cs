namespace AdventOfCode2024.Helpers
{
    internal class IntSequence()
    {
        private int current;
        
        public void Set(int value)
        {
            current = value;
        }

        public void Reset()
        {
            current = 0;
        }

        public int Next()
        {
            current++;
            return current;
        }

        public int Peek()
        {
            return current+1;
        }
    }
    internal class Namer
    {
        private List<string> names;
        private char[] letters;
        private int current;
        public Namer()
        {
            names = new List<string>(File.ReadAllLines("Helpers/names.txt"));
            letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzαβγδεζηθικλμνξοπρστυφψω".ToCharArray();
            current = 0;
        }

        public string NextName()
        {
            var r = names[current];
            current++;
            return r;
        }

        public string NextLetter()
        {
            var i = current;
            string r = "";
            if (i / letters.Length > 0)
            {
                var j = i / letters.Length;
                if (j / letters.Length > 0)
                {
                    r += letters[j / letters.Length - 1];
                    i = i % letters.Length;
                }
                r += letters[j % letters.Length];
            }
            r += letters[i % letters.Length];
            current++;
            return r;
        }

        public void Reset()
        {
            current = 0;
        }
    }
}

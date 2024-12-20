using AdventOfCode2024.Helpers;
using System.Data;
using System.Drawing;

namespace AdventOfCode2024
{
    internal class Day15
    {
        const bool TEST = false;

        internal record struct Box(int x, int y1, int y2)
        {
            public static implicit operator (int x, int y1, int y2)(Box value)
            {
                return (value.x, value.y1, value.y2);
            }

            public static implicit operator Box((int x, int y1, int y2) value)
            {
                return new Box(value.x, value.y1, value.y2);
            }
        }

        private char[,] map;
        private List<dirs> moves = new List<dirs>();

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 15 !! ##################################\r\n");
            long total1 = 0;
            long total2 = 0;
            //Read file
            var path = $@"{(TEST ? "Samples" : "Inputs")}\day15.txt";
            var path2 = $@"{(TEST ? "Samples" : "Inputs")}\day15-2.txt";
            map = MapHelper.LoadCharMap(path);
            var lines = File.ReadAllLines(path2);
            foreach (var line in lines)
            {
                moves.AddRange(line.ToCharArray().Select(GetDirection));
            }

            Point start = map.AsPointEnumerable().FirstOrDefault(p => map.Get(p) == '@');
            Point current = start;
            foreach (var move in moves)
            {
                current = ProcessMove(current, move);
            }
            total1 = map.AsPointEnumerable().Where(p => map.Get(p) == 'O').Select(p => p.X * 100 + p.Y).Sum();


            map = CustomLoadCharMap(path);
            start = map.AsPointEnumerable().FirstOrDefault(p => map.Get(p) == '@');
            current = start;
            foreach (var move in moves)
            {
                current = ProcessMove2(current, move);
            }
            map.Print();
            total2 = map.AsPointEnumerable().Where(p => map.Get(p) == '[').Select(p => p.X * 100 + p.Y).Sum();

            //Print out total result
            Console.WriteLine(
$@"Final result for 1st star is : {total1}
Final result for 2nd star is : {total2}
**************************** END OF DAY 15 ***********************************");
            Thread.Sleep(1000);
        }

        private static dirs GetDirection(char c)
        {
            switch (c)
            {
                case '>': return dirs.east;
                case '<': return dirs.west;
                case 'v': return dirs.south;
                case '^': return dirs.north;
                default: return dirs.north;
            }
        }

        private Point ProcessMove(Point current, dirs move)
        {
            var nextPos = current.Move(move);
            var nextVal = map.Get(nextPos);
            //If the next spot is empty, then just move
            if (nextVal == '.')
            {
                map.Set(current, '.');
                map.Set(nextPos, '@');
                return nextPos;
            }
            else if (nextVal == 'O')
            {
                //Find what's on the other side of the boxes
                var p = nextPos.Move(move);
                while (map.Get(p) == 'O')
                {
                    p = p.Move(move);
                }
                if (map.Get(p) == '#') return current; // nothing moves
                if (map.Get(p) == '.')
                {
                    map.Set(p, 'O');
                    map.Set(current, '.');
                    map.Set(nextPos, '@');
                    return nextPos;
                }
            }
            //Else, there is a wall, do not move
            return current;
        }


        //########################################## 2ND STAR ########################################################
        private char[,] CustomLoadCharMap(string path)
        {
            var lines = File.ReadAllLines(path);
            var map = new char[lines.Length, lines[0].Length * 2];
            for (int i = 0; i < lines.Length; i++)
            {
                var k = 0;
                for (int j = 0; j < lines[i].Length; j++)
                {
                    switch (lines[i][j])
                    {
                        case '.':
                        case '#':
                            map[i, k] = lines[i][j]; map[i, k + 1] = lines[i][j];
                            break;
                        case '@':
                            map[i, k] = '@'; map[i, k + 1] = '.';
                            break;
                        case 'O':
                            map[i, k] = '['; map[i, k + 1] = ']';
                            break;
                    }
                    k += 2;
                }
            }
            return map;
        }

        private Point ProcessMove2(Point current, dirs move)
        {
            var nextPos = current.Move(move);
            var nextVal = map.Get(nextPos);
            //If the next spot is empty, then just move
            if (nextVal == '.')
            {
                map.Set(current, '.');
                map.Set(nextPos, '@');
                return nextPos;
            }
            else if (nextVal == '#')
            {
                //Blocked
                return current;
            }
            else
            {
                //Recursively find out the list of boxes that will move
                var boxesThatWillMove = BoxAndNexts(nextPos, move);
                if (boxesThatWillMove == null)
                {
                    return current; //Blocked along the way
                }
                //Process them in the right order as not to mess up the formating
                //We have to apply a distinct as well since we could end up moving a box from two different paths.
                var boxes = boxesThatWillMove.Distinct().OrderBy(b =>
                {
                    switch (move)
                    {
                        case dirs.north: return b.x;
                        case dirs.south: return -b.x;
                        case dirs.west: return b.y1;
                        case dirs.east: return -b.y2;
                    }
                    return 0;
                }).ToList();
                foreach (var b in boxes) { MoveBox(b, move); }
                map.Set(current, '.');
                map.Set(nextPos, '@');
                return nextPos;
            }
        }

        private void MoveBox(Box b, dirs move)
        {
            switch (move)
            {
                case dirs.north:
                    map[b.x - 1, b.y1] = '[';
                    map[b.x - 1, b.y2] = ']';
                    map[b.x, b.y1] = '.';
                    map[b.x, b.y2] = '.';
                    break;
                case dirs.south:
                    map[b.x + 1, b.y1] = '[';
                    map[b.x + 1, b.y2] = ']';
                    map[b.x, b.y1] = '.';
                    map[b.x, b.y2] = '.';
                    break;
                case dirs.west:
                    map[b.x, b.y1-1] = '[';
                    map[b.x, b.y2-1] = ']';
                    map[b.x, b.y2] = '.';
                    break;
                case dirs.east:
                    map[b.x, b.y1 + 1] = '[';
                    map[b.x, b.y2 + 1] = ']';
                    map[b.x, b.y1] = '.';
                    break;
            }
        }

        private List<Box> BoxAndNexts(Point current, dirs move)
        {
            var val = map.Get(current);
            Box box;
            if (val == '[')
            {
                box = (current.X, current.Y, current.Y + 1);
            }
            else
            {
                box = (current.X, current.Y - 1, current.Y);
            }
            var res = new List<Box> { box };
            var p1 = new Point(box.x, box.y1).Move(move);
            var p2 = new Point(box.x, box.y2).Move(move);
            var v1 = map.Get(p1);
            var v2 = map.Get(p2);
            if (IsBlocked(v1, v2, move))
            {
                //Blocked => Nothing moves
                return null;
            }
            else if (IsFree(v1, v2, move))
            {
                return res;
            }
            else if (PushesOneBox(v1, v2, move))
            {
                List<Box> nexts;
                if (move == dirs.east)
                {
                    //When moving east, the next box starts in p2.
                    nexts = BoxAndNexts(p2, move);
                }
                else if ( v1 == '.')
                {
                    //When moving north or south and the next box is a little to the right then
                    //the next box start in p2.
                    nexts = BoxAndNexts(p2, move);
                }
                else
                {
                    //For all other cases, the next box starts in p1.
                    nexts = BoxAndNexts(p1, move);
                }
                if (nexts == null) return null;
                res.AddRange(nexts);
            }
            else if (PushesTwoBoxes(v1, v2, move))
            {
                //When moving two boxes (only when moving north or south), one start on p1 and the other in p2.
                var nexts = BoxAndNexts(p1, move);
                if (nexts == null) return null;
                res.AddRange(nexts);
                nexts = BoxAndNexts(p2, move);
                if (nexts == null) return null;
                res.AddRange(nexts);
            }
            return res;
        }

        private bool PushesOneBox(char v1, char v2, dirs move)
        {
            switch (move)
            {
                case dirs.west: return v1 == ']';
                case dirs.east: return v2 == '[';
                case dirs.north:
                case dirs.south: return (v1 == '[' && v2 == ']')
                        || (v1 == '.' && v2 == '[')
                        || (v1 == ']' && v2 == '.');
            }
            return false;
        }

        private bool PushesTwoBoxes(char v1, char v2, dirs move)
        {
            switch (move)
            {
                case dirs.west:
                case dirs.east: return false;
                case dirs.north:
                case dirs.south: return v1 == ']' && v2 == '[';
            }
            return false;
        }

        private bool IsFree(char v1, char v2, dirs move)
        {
            switch (move)
            {
                case dirs.west: return v1 == '.';
                case dirs.east: return v2 == '.';
                case dirs.north:
                case dirs.south: return v1 == '.' && v2 == '.';
            }
            return false;
        }

        private bool IsBlocked(char v1, char v2, dirs move)
        {
            switch (move)
            {
                case dirs.west: return v1 == '#';
                case dirs.east: return v2 == '#';
                case dirs.north:
                case dirs.south: return v1 == '#' || v2 == '#';
            }
            return false;
        }


    }


}



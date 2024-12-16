using System.Collections;
using System.Drawing;

namespace AdventOfCode2024.Helpers
{
    internal static class MapHelper
    {
        public static char[,] LoadCharMap(string path)
        {
            var lines = File.ReadAllLines(path);
            var map = new char[lines.Length, lines[0].Length];
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    map[i, j] = lines[i][j];
                }
            }
            return map;
        }

        public static int[,] LoadIntMap(string path)
        {
            var lines = File.ReadAllLines(path);
            var map = new int[lines.Length, lines[0].Length];
            for (int i = 0; i < lines.Length; i++)
            {
                var r = lines[i];
                for (int j = 0; j < lines[i].Length; j++)
                {

                    map[i, j] = int.Parse(new string(r[j],1));
                }
            }
            return map;
        }
    }

    internal static class MapExtensions
    {
        public static T Get<T>(this T[,] map, Point p)
        {
            return map[p.X, p.Y];
        }

        public static void Set<T>(this T[,] map, Point p,T value)
        {
            map[p.X, p.Y]=value;
        }

        public static void ForEachIndex<T>(this T[,] map, Action<Point> action)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    action(new Point(i, j));
                }
            }
        }
        public static void ForEach<T>(this T[,] map, Action<T> action)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    action(map[i, j]);
                }
            }
        }

        public static void ForEachRow<T>(this T[,] map, Action<int> action)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                action(i);
            }
        }

        public static void ForEachCol<T>(this T[,] map, Action<int> action)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                action(j);
            }
        }

        public static IEnumerator<T> GetEnumerator<T>(this T[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    yield return map[i, j];
                }
            }
        }

        public static IEnumerator<Point> GetPointEnumerator<T>(this T[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    yield return new Point(i,j);
                }
            }
        }

        public static IEnumerable<T> AsEnumerable<T>(this T[,] map)
        {
            return new EnumerableWrapper<T>(map);
        }

        public static IEnumerable<Point> AsPointEnumerable<T>(this T[,] map)
        {
            return new PointEnumerableWrapper<T>(map);
        }

        public static bool IsOutOfBound<T>(this T[,] map, Point p, int offset = 0)
        {
            return map.IsOutOfBound(p.X, p.Y,offset);
        }

        public static bool IsOutOfBound<T>(this T[,] map,int i, int j,int offset=0) {
            if (i < 0+offset || j < 0 + offset || i >= map.GetLength(0)-offset || j >= map.GetLength(1)-offset) return true;
            return false;
        }

        public static void Print<T>(this T[,] map,Predicate<T>? p=null,ConsoleColor color=ConsoleColor.White)
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if(p!=null && p(map[i, j]))
                    {
                        Console.ForegroundColor = color;
                    }
                    Console.Write(map[i, j]);
                    Console.ResetColor();
                }
                Console.Write(Environment.NewLine);
            }
        }
    }

    public class EnumerableWrapper<T> : IEnumerable<T>
    {
        private readonly T[,] _map;
        public EnumerableWrapper(T[,] map)
        {
            _map = map;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _map.GetEnumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.GetEnumerator<T>();
        }
    }

    public class PointEnumerableWrapper<T> : IEnumerable<Point>
    {
        private readonly T[,] _map;
        public PointEnumerableWrapper(T[,] map)
        {
            _map = map;
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return _map.GetPointEnumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.GetPointEnumerator<T>();
        }
    }
}

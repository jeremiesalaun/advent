using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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

        public static IEnumerable<T> AsEnumerable<T>(this T[,] map)
        {
            return new EnumerableWrapper<T>(map);
        }

        public static bool IsOutOfBound<T>(this T[,] map, Point p, int offset = 0)
        {
            return map.IsOutOfBound(p.X, p.Y,offset);
        }

        public static bool IsOutOfBound<T>(this T[,] map,int i, int j,int offset=0) {
            if (i < 0+offset || j < 0 + offset || i >= map.GetLength(0)-offset || j >= map.GetLength(1)-offset) return true;
            return false;
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
}

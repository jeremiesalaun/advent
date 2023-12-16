using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
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
        public static void ForEach<T>(this T[,] map , Action<T> action)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for(int j = 0; j < map.GetLength(1); j++)
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
    }

    public class EnumerableWrapper<T> : IEnumerable<T>
    {
        private readonly T[,] _map;
        public EnumerableWrapper(T[,]map) {
            this._map = map;
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

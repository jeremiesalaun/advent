using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Helpers
{
    public enum Directions { none,north, south, west, east }
    internal static class NavigationHelper
    {
        public static Directions TurnRight(Directions d)
        {
            switch (d)
            {
                case Directions.north: return Directions.east;
                case Directions.south: return Directions.west;
                case Directions.east: return Directions.south;
                case Directions.west: return Directions.north;
                default: return Directions.none;
            }
        }

        public static Directions TurnLeft(Directions d)
        {
            switch (d)
            {
                case Directions.north: return Directions.west;
                case Directions.south: return Directions.east;
                case Directions.east: return Directions.north;
                case Directions.west: return Directions.south;
                default: return Directions.none;
            }
        }
    }
    internal static class PointExtensions
    {
        public static Point Move(this Point p, Directions d)
        {
            Point r;
            switch (d)
            {
                case Directions.north: r = new Point(p.X - 1, p.Y); break;
                case Directions.south: r = new Point(p.X + 1, p.Y); break;
                case Directions.east: r = new Point(p.X, p.Y + 1); break;
                case Directions.west: r = new Point(p.X, p.Y - 1); break;
                default: r = p; break;
            }
            return r;
        }
    }
}

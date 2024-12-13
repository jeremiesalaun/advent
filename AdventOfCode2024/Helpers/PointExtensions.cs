using System.Drawing;

namespace AdventOfCode2024.Helpers
{
    public enum dirs { none, north, south, west, east }
    internal static class NavigationHelper
    {
        public static dirs TurnRight(dirs d)
        {
            switch (d)
            {
                case dirs.north: return dirs.east;
                case dirs.south: return dirs.west;
                case dirs.east: return dirs.south;
                case dirs.west: return dirs.north;
                default: return dirs.none;
            }
        }

        public static dirs TurnLeft(dirs d)
        {
            switch (d)
            {
                case dirs.north: return dirs.west;
                case dirs.south: return dirs.east;
                case dirs.east: return dirs.north;
                case dirs.west: return dirs.south;
                default: return dirs.none;
            }
        }
    }
    internal static class PointExtensions
    {
        public static Point Move(this Point p, dirs d)
        {
            Point r;
            switch (d)
            {
                case dirs.north: r = new Point(p.X - 1, p.Y); break;
                case dirs.south: r = new Point(p.X + 1, p.Y); break;
                case dirs.east: r = new Point(p.X, p.Y + 1); break;
                case dirs.west: r = new Point(p.X, p.Y - 1); break;
                default: r = p; break;
            }
            return r;
        }

        public static Point Substract(this Point p, Point value)
        {
            return new Point(p.X-value.X, p.Y-value.Y);
        }

        public static Point Mult(this Point p, int multiplicator)
        {
            return new Point(p.X * multiplicator, p.Y * multiplicator);
        }
        public static Point Add(this Point p, Point value)
        {
            return new Point(p.X + value.X, p.Y + value.Y);
        }
    }
}

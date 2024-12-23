﻿using System.Diagnostics.Metrics;
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

        public static dirs Opposite(dirs d)
        {
            switch (d)
            {
                case dirs.west: return dirs.east;
                case dirs.north: return dirs.south;
                case dirs.south: return dirs.north;
                case dirs.east: return dirs.west;
            }
            return dirs.none;
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

        public static int DistanceTo(this Point p, Point to)
        {
            return Math.Abs(p.X - to.X)+ Math.Abs(p.Y - to.Y);
        }

        public static Point Substract(this Point p, Point value)
        {
            return new Point(p.X-value.X, p.Y-value.Y);
        }

        public static Point Wrap(this Point p, Point bounds)
        {
            var q = new Point(p.X % bounds.X, p.Y % bounds.Y);
            if (q.X < 0) q.X = bounds.X + q.X;
            if (q.Y < 0) q.Y = bounds.Y + q.Y;
            return q;
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

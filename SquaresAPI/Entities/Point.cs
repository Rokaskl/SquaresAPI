using System;
using Microsoft.EntityFrameworkCore;

namespace SquaresAPI.Entities
{
    [Owned]
    public class Point
    {
        public Point()
        {
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not Point point)
                return false;

            return X == point.X && Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
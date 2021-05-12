using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SquaresAPI.Entities
{
    [Owned]
    public class Square
    {
        public List<Point> Points { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not Square square) return false;

            return Points.Intersect(square.Points).Count() == Points.Count;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Points);
        }
    }
}
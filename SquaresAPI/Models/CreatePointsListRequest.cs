using System.Collections.Generic;
using SquaresAPI.Entities;

namespace SquaresAPI.Models
{
    public class CreatePointsListRequest
    {
        public IEnumerable<Point> Points { get; set; }
    }
}
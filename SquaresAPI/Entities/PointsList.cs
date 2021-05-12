using System.Collections.Generic;

namespace SquaresAPI.Entities
{
    public class PointsList
    {
        public PointsList()
        {
            IsSquaresUpdateNeeded = true;
        }

        public int Id { get; set; }
        public List<Point> Points { get; set; }
        public List<Square> Squares { get; set; }
        public bool IsSquaresUpdateNeeded { get; set; }
    }
}
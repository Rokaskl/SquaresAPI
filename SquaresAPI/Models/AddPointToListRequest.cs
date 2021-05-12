namespace SquaresAPI.Models
{
    public class AddPointToListRequest
    {
        public int PointsListId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
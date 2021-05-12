using Microsoft.EntityFrameworkCore;
using SquaresAPI.Entities;

namespace SquaresAPI.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<PointsList> PointsLists { get; set; }
    }
}
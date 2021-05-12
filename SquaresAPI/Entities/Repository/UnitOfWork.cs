using SquaresAPI.Data;

namespace SquaresAPI.Entities.Repository
{
    public interface IUnitOfWork
    {
        IPointsListRepository PointsListRepository { get; }
        void Commit();
        void Rollback();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        private IPointsListRepository _pointsListRepository;

        public UnitOfWork(Context context)
        {
            _context = context;
        }

        public IPointsListRepository PointsListRepository
        {
            get { return _pointsListRepository ??= new PointsListRepository(_context); }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Rollback()
        {
            _context.Dispose();
        }
    }
}
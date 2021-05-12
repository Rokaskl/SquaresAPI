using System.Collections.Generic;
using System.Linq;
using SquaresAPI.Data;

namespace SquaresAPI.Entities.Repository
{
    public interface IPointsListRepository : IDataRepository<PointsList>
    {
        void RemovePoint(PointsList entity, Point point);
        void AddPoint(PointsList entity, Point point);
        void UpdateSquares(PointsList entity, List<Square> squares);
        void AddSquares(PointsList entity, List<Square> squares);
    }

    public class PointsListRepository : IPointsListRepository
    {
        private readonly Context _Context;

        public PointsListRepository(Context context)
        {
            _Context = context;
        }

        public IEnumerable<PointsList> GetAll()
        {
            return _Context.PointsLists.ToList();
        }

        public PointsList Get(int id)
        {
            return _Context.PointsLists.FirstOrDefault(e => e.Id == id);
        }

        public void Add(PointsList entity)
        {
            _Context.PointsLists.Add(entity);
        }

        public void Update(PointsList entity, PointsList updatedEntity)
        {
            entity.Points = updatedEntity.Points;
            entity.Squares = updatedEntity.Squares;
            entity.IsSquaresUpdateNeeded = true;
        }

        public void RemovePoint(PointsList entity, Point point)
        {
            entity.Points.Remove(point);
            entity.IsSquaresUpdateNeeded = true;
        }

        public void AddPoint(PointsList entity, Point point)
        {
            entity.Points.Add(point);
            entity.IsSquaresUpdateNeeded = true;
        }

        public void Delete(PointsList entity)
        {
            _Context.PointsLists.Remove(entity);
        }

        public void UpdateSquares(PointsList entity, List<Square> squares)
        {
            entity.Squares = squares;
            entity.IsSquaresUpdateNeeded = false;
        }

        public void AddSquares(PointsList entity, List<Square> squares)
        {
            entity.Squares.AddRange(squares);
            entity.IsSquaresUpdateNeeded = false;
        }
    }
}
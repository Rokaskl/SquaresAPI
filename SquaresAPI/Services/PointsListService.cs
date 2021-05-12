using System;
using System.Collections.Generic;
using System.Linq;
using SquaresAPI.Entities;
using SquaresAPI.Entities.Repository;
using SquaresAPI.Exceptions;
using SquaresAPI.Models;

namespace SquaresAPI.Services
{
    public interface IPointsListService
    {
        IEnumerable<PointsList> GetAll();
        PointsList Get(int id);
        PointsList AddPoint(AddPointToListRequest model);
        PointsList RemovePoint(RemovePointFromListRequest model);
        PointsList Create(CreatePointsListRequest model);
        void DeletePointsList(int id);
        List<Square> GetSquares(int pointsListId);
    }

    public class PointsListService : IPointsListService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PointsListService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PointsList AddPoint(AddPointToListRequest model)
        {
            var pointsList = _unitOfWork.PointsListRepository.Get(model.PointsListId);
            if (pointsList == null) throw new NotFoundException("Points list does not exist");

            var newPoint = new Point {X = model.X, Y = model.Y};

            if (pointsList.Points.Contains(newPoint))
                throw new AlreadyExistsException("Point already exists in points list");

            _unitOfWork.PointsListRepository.AddPoint(pointsList, newPoint);

            //O(points.Count) now instead of O(points.Count^2) later 
            _unitOfWork.PointsListRepository.AddSquares(pointsList,
                FindSquaresAfterAddedPoint(pointsList.Points, newPoint));
            _unitOfWork.Commit();
            return pointsList;
        }

        public PointsList Create(CreatePointsListRequest model)
        {
            var newPointsList = new PointsList
            {
                Points = new List<Point>()
            };
            if (model.Points != null && model.Points.Any())
                foreach (var point in model.Points)
                    if (!newPointsList.Points.Contains(point))
                        newPointsList.Points.Add(point);
            _unitOfWork.PointsListRepository.Add(newPointsList);
            _unitOfWork.Commit();
            return newPointsList;
        }

        public void DeletePointsList(int id)
        {
            var pointsList = _unitOfWork.PointsListRepository.Get(id);
            if (pointsList == null) throw new NotFoundException("Points list does not exist");

            _unitOfWork.PointsListRepository.Delete(pointsList);
            _unitOfWork.Commit();
        }

        public PointsList Get(int id)
        {
            return _unitOfWork.PointsListRepository.Get(id);
        }

        public IEnumerable<PointsList> GetAll()
        {
            return _unitOfWork.PointsListRepository.GetAll();
        }

        public PointsList RemovePoint(RemovePointFromListRequest model)
        {
            var pointsList = _unitOfWork.PointsListRepository.Get(model.PointsListId);
            if (pointsList == null) throw new NotFoundException("Points list does not exist");

            var pointToRemove = new Point {X = model.X, Y = model.Y};

            if (!pointsList.Points.Contains(pointToRemove))
                throw new NotFoundException("Point does not exist in the list");

            _unitOfWork.PointsListRepository.RemovePoint(pointsList, pointToRemove);
            //O(Squares.Count) now instead of O(points.Count^2) later
            _unitOfWork.PointsListRepository.UpdateSquares(pointsList,
                RemoveSquaresAfterDeletedPoint(pointsList.Squares, pointToRemove));
            _unitOfWork.Commit();

            return pointsList;
        }

        public List<Square> GetSquares(int pointsListId)
        {
            var pointsList = _unitOfWork.PointsListRepository.Get(pointsListId);

            if (pointsList == null) throw new NotFoundException("Points list does not exist");

            //Can return already calculated squares
            if (!pointsList.IsSquaresUpdateNeeded) return pointsList.Squares;

            var newSquares = FindSquares(pointsList.Points);
            _unitOfWork.PointsListRepository.UpdateSquares(pointsList, newSquares);
            _unitOfWork.Commit();

            return newSquares;
        }

        //helpers
        public virtual List<Square> FindSquares(List<Point> points)
        {
            var squares = new List<Square>();

            //For ~O(1) .Contains() 
            var set = new HashSet<Point>();
            foreach (var point in points) set.Add(point);
            for (var i = 0; i < points.Count - 1; i++)
            for (var j = i + 1; j < points.Count; j++)
            {
                //For each Point points[i], points[j], check if PossiblePairs points exist in set.
                var possiblePairs = FindPossiblePointsPairsToFormSquare(points[i], points[j]);
                foreach (var (item1, item2) in possiblePairs)
                    if (set.Contains(item1) && set.Contains(item2))
                    {
                        var newSquare = new Square
                            {Points = new List<Point> {points[i], points[j], item1, item2}};

                        if (squares.Contains(newSquare)) continue;

                        squares.Add(newSquare);
                    }
            }

            return squares;
        }

        public virtual List<Square> FindSquaresAfterAddedPoint(List<Point> points, Point newPoint)
        {
            var squares = new List<Square>();

            //For ~O(1) .Contains() 
            var set = new HashSet<Point>();
            foreach (var point in points) set.Add(point);
            for (var i = 0; i < points.Count; i++)
            {
                //Same point
                if (newPoint.Equals(points[i])) continue;

                //For each Point points[i] and newPoint, check if PossiblePairs points exist in set.
                var possiblePairs = FindPossiblePointsPairsToFormSquare(points[i], newPoint);
                foreach (var (item1, item2) in possiblePairs)
                    if (set.Contains(item1) && set.Contains(item2))
                    {
                        var newSquare = new Square
                            {Points = new List<Point> {points[i], newPoint, item1, item2}};

                        if (squares.Contains(newSquare)) continue;

                        squares.Add(newSquare);
                    }
            }

            return squares;
        }

        public virtual List<Square> RemoveSquaresAfterDeletedPoint(List<Square> squares, Point deletedPoint)
        {
            var newSquares = squares.Where(s => !s.Points.Contains(deletedPoint)).ToList();
            return newSquares;
        }

        /// <summary>
        ///     Calculate and return two possible pairs of points that can form a square
        /// </summary>
        /// <param name="a">Point a</param>
        /// <param name="b">Point b</param>
        /// <returns> Two possible pairs of points that can form a square </returns>
        public virtual List<Tuple<Point, Point>> FindPossiblePointsPairsToFormSquare(Point a, Point b)
        {
            //https://www.youtube.com/watch?v=gib7vkeEIR4 video explains how this algorithm works
            var deltaX = b.X - a.X;
            var deltaY = b.Y - a.Y;

            var c1 = new Point {X = a.X - deltaY, Y = a.Y + deltaX};
            var d1 = new Point {X = b.X - deltaY, Y = b.Y + deltaX};

            var c2 = new Point {X = a.X + deltaY, Y = a.Y - deltaX};
            var d2 = new Point {X = b.X + deltaY, Y = b.Y - deltaX};

            var firstSet = new Tuple<Point, Point>(c1, d1);
            var secondSet = new Tuple<Point, Point>(c2, d2);

            return new List<Tuple<Point, Point>> {firstSet, secondSet};
        }
    }
}
using Moq;
using NUnit.Framework;
using SquaresAPI.Entities;
using SquaresAPI.Entities.Repository;
using SquaresAPI.Models;
using SquaresAPI.Services;
using System;
using System.Collections.Generic;

namespace SquaresAPItests.Services
{
    [TestFixture]
    public class PointsListServiceTests
    {
        private List<Point> onePoint = new List<Point> { new Point { X = 0, Y = 0 } };
        private List<Point> threePoints = new List<Point> { new Point { X = 0, Y = 0 }, new Point { X = 1, Y = 0 }, new Point { X = 1, Y = 1 } };
        private List<Point> twoOverlapedSquaresPoints = 
            new List<Point> { new Point { X = 0, Y = 0 }, new Point { X = 0, Y = 1 }, new Point { X = -1, Y = 1 },
                              new Point { X = -1, Y = 0 }, new Point { X = -2, Y = 0 }, new Point { X = -1, Y = -1 } };
        private List<Square> twoOverlapedSquares = new List<Square> {
            new Square { Points = new List<Point> { new Point { X = 0, Y = 0 }, 
                new Point { X = 0, Y = 1 }, new Point { X = -1, Y = 1 }, new Point { X = -1, Y = 0 } } },
            new Square { Points = new List<Point> { new Point { X = 0, Y = 0 }, 
                new Point { X = -1, Y = 1 }, new Point { X = -2, Y = 0 }, new Point { X = -1, Y = -1 } } }
            };
        private Point threePointsAddition = new Point { X = 0, Y = 1 };
        private PointsList mockedPointslistObj;
        private MockRepository mockRepository;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<IPointsListRepository> mockPointsListRepository;

        [SetUp]
        public void SetUp()
        {
            mockedPointslistObj = new PointsList { Id = 1, Points = twoOverlapedSquaresPoints, Squares = twoOverlapedSquares, IsSquaresUpdateNeeded = false };
            mockRepository = new MockRepository(MockBehavior.Strict);
            mockPointsListRepository = this.mockRepository.Create<IPointsListRepository>();
            mockUnitOfWork = this.mockRepository.Create<IUnitOfWork>();
            mockPointsListRepository.Setup(r => r.Get(It.IsAny<int>())).Returns(mockedPointslistObj);
            mockUnitOfWork.SetupGet(u => u.PointsListRepository).Returns(this.mockPointsListRepository.Object);
            mockUnitOfWork.Setup(u => u.Commit());
        }

        private PointsListService CreateService()
        {
            return new PointsListService(this.mockUnitOfWork.Object);
        }

        private Mock<PointsListService> CreateServiceMock()
        {
            return new Mock<PointsListService>(this.mockUnitOfWork.Object); 
            
        }

        [Test]
        public void AddPoint_PointDoesNotExist_CallsRepoMethod()
        {
            // Arrange
            AddPointToListRequest addPointToListRequest = new AddPointToListRequest { PointsListId = 1, X = 10, Y = 10 };
            var mockedService = this.CreateServiceMock();
            mockedService.Setup(s => s.FindSquaresAfterAddedPoint(It.IsAny<List<Point>>(), It.IsAny<Point>())).Returns(new List<Square>());
            var expected = new List<Point> { new Point { X = 0, Y = 0 }, new Point { X = 0, Y = 1 }, new Point { X = -1, Y = 1 }, new Point { X = -1, Y = 0 }, new Point { X = -2, Y = 0 }, new Point { X = -1, Y = -1 }, new Point { X = 10, Y = 10 } };
            mockPointsListRepository.Setup(r => r.AddPoint(It.IsAny<PointsList>(), It.IsAny<Point>()));
            mockPointsListRepository.Setup(r => r.AddSquares(It.IsAny<PointsList>(), It.IsAny<List<Square>>()));

            // Act
            var result = mockedService.Object.AddPoint(addPointToListRequest);

            // Assert
            mockPointsListRepository.Verify(r => r.AddPoint(It.IsAny<PointsList>(), It.IsAny<Point>()), Times.Exactly(1));
            mockedService.Verify(s => s.FindSquaresAfterAddedPoint(It.IsAny<List<Point>>(), It.IsAny<Point>()), Times.Exactly(1));

        }

        [Test]
        public void Create_NoDuplicatedPoints_ReturnsCreatedPointsListObjWithAllPoints()
        {
            // Arrange
            var service = this.CreateService();
            CreatePointsListRequest model =  new CreatePointsListRequest { Points = threePoints };
            mockPointsListRepository.Setup(r => r.Add(It.IsAny<PointsList>()));

            // Act
            var result = service.Create(model);

            // Assert
            mockPointsListRepository.Verify(r => r.Add(It.IsAny<PointsList>()), Times.Exactly(1));

        }

        [Test]
        public void DeletePointsList_PointsListExists_CallRepoMethod()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            mockPointsListRepository.Setup(r => r.Delete(It.IsAny<PointsList>()));

            // Act
            service.DeletePointsList(id);

            // Assert
            mockPointsListRepository.Verify(x => x.Delete(It.IsAny<PointsList>()), Times.Exactly(1));

        }

        [Test]
        public void Get_Exists_ReturnFoundPointsList()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            mockPointsListRepository.Setup(r => r.Get(1)).Returns(mockedPointslistObj);
            var expected = mockedPointslistObj;

            // Act
            var result = service.Get(id);

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void GetAll_TwoExists_ReturnTwo()
        {
            // Arrange
            var service = this.CreateService();
            var expected = new List<PointsList> { new PointsList(), new PointsList() };
            mockPointsListRepository.Setup(r => r.GetAll()).Returns(expected);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void RemovePoint_PointExists_CallsRepoMethod()
        {
            // Arrange
            var mockedService = this.CreateServiceMock();
            mockedService.Setup(s => s.RemoveSquaresAfterDeletedPoint(It.IsAny<List<Square>>(), It.IsAny<Point>())).Returns(new List<Square>());
            mockPointsListRepository.Setup(r => r.RemovePoint(It.IsAny<PointsList>(),It.IsAny<Point>()));
            mockPointsListRepository.Setup(r => r.UpdateSquares(It.IsAny<PointsList>(), It.IsAny<List<Square>>()));
            RemovePointFromListRequest model = new RemovePointFromListRequest { PointsListId = 1, X = 0, Y = 0};

            // Act
            var result = mockedService.Object.RemovePoint(model);

            // Assert
            mockPointsListRepository.Verify(x => x.RemovePoint(It.IsAny<PointsList>(), new Point { X=0, Y=0 }), Times.Exactly(1));
            mockedService.Verify(x => x.RemoveSquaresAfterDeletedPoint(It.IsAny<List<Square>>(), It.IsAny<Point>()), Times.Exactly(1));
        }

        [Test]
        public void GetSquares_UpdateFlagFalse_ReturnPrecalculatedSquares()
        {
            // Arrange
            var service = this.CreateService();
            int pointsListId = 0;

            // Act
            var result = service.GetSquares(pointsListId);
            var expected = twoOverlapedSquares;
            // Assert
            Assert.AreEqual(expected, result);

        }
        [Test]
        public void FindSquares_OnePoint_ReturnEmptyList()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = service.FindSquares(onePoint);

            // Assert
            Assert.AreEqual(new List<Square>(), result);

        }
        [Test]
        public void FindSquares_ThreePoints_ReturnEmptyList()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = service.FindSquares(threePoints);

            // Assert
            Assert.AreEqual(new List<Square>(), result);

        }
        [Test]
        public void FindSquares_TwoOverlapedSquares_ReturnListWithTwoSquares()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = service.FindSquares(twoOverlapedSquaresPoints);

            // Assert
            CollectionAssert.AreEquivalent(twoOverlapedSquares, result);
        }

        [Test]
        public void FindSquaresAfterAddedPoint_AddingOnePointThatFormsNewSquare_ReturnNewSquare()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = service.FindSquaresAfterAddedPoint(threePoints, threePointsAddition);

            // Assert
            List<Point> expectedNewSquarePoints = new List<Point>();
            expectedNewSquarePoints.AddRange(threePoints);
            expectedNewSquarePoints.Add(threePointsAddition);

            List<Square> expected = new List<Square> { new Square { Points = expectedNewSquarePoints } };
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void RemoveSquaresAfterDeletedPoint_RemovePointOfOneSquareFromOverlapedSquares_ReturnOneSquare()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = service.RemoveSquaresAfterDeletedPoint(twoOverlapedSquares, new Point { X = -2, Y = 0 });

            // Assert
            List<Square> expected = new List<Square>();
            expected.Add(twoOverlapedSquares[0]);
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void FindPossiblePointsPairsToFormSquare_VerticalPoints_ReturnTwoPairs()
        {
            // Arrange
            var service = this.CreateService();
            Point a = new Point { X = -100, Y = 10 };
            Point b = new Point { X = 100, Y = 10 };

            // Act
            var result = service.FindPossiblePointsPairsToFormSquare(a, b);
            var expected = new List<Tuple<Point, Point>> { new Tuple<Point, Point>(new Point { X = -100, Y = 210 }, new Point { X = 100, Y = 210 }),
                new Tuple<Point, Point>(new Point { X = -100, Y = -190 }, new Point { X = 100, Y = -190 }) } ;
            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SquaresAPI.Controllers;
using SquaresAPI.Entities;
using SquaresAPI.Models;
using SquaresAPI.Services;
using SquaresAPI.Exceptions;
using System;
using System.Collections.Generic;

namespace SquaresAPItests.Controllers
{
    [TestFixture]
    public class PointsListControllerTests
    {
        PointsList mockedPointsListEmpty;
        PointsList mockedPointsListOneSquare;
        List<Point> simpleSquare = 
            new List<Point> { new Point { X = 0, Y = 1 }, new Point { X = 0, Y = 0 }, new Point { X = 1, Y = 0 }, new Point { X = 1, Y = 1 } };
        RemovePointFromListRequest validRemovePointFromListRequest = new RemovePointFromListRequest { PointsListId = 1, X = 1, Y = 1 };
        RemovePointFromListRequest noListRemovePointFromListRequest = new RemovePointFromListRequest { PointsListId = 0, X = 1, Y = 1 };
        RemovePointFromListRequest noPointRemovePointFromListRequest = new RemovePointFromListRequest { PointsListId = 1, X = 0, Y = 0 };
        CreatePointsListRequest validCreatePointsListRequest;
        AddPointToListRequest validAddPointToListRequest = new AddPointToListRequest { PointsListId = 1, X = 1, Y = 1 };
        AddPointToListRequest listDoesNotExistAddPointToListRequest = new AddPointToListRequest { PointsListId = 0, X = 1, Y = 1 };

        private MockRepository mockRepository;
        private Mock<IPointsListService> mockPointsListService;

        [SetUp]
        public void SetUp()
        {
            mockedPointsListEmpty= new PointsList { Id = 1, Points = new List<Point>(), Squares = 
                new List<Square>(), IsSquaresUpdateNeeded = false };
            mockedPointsListOneSquare = new PointsList { Id = 2, Points = simpleSquare, Squares =
                new List<Square>() { new Square { Points = simpleSquare } }, IsSquaresUpdateNeeded = false };
            validCreatePointsListRequest = new CreatePointsListRequest { Points = simpleSquare };
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockPointsListService = this.mockRepository.Create<IPointsListService>();

        }

        private PointsListController CreatePointsListController()
        {
            return new PointsListController(this.mockPointsListService.Object);
        }

        [Test]
        public void Get_ExistingPointList_ReturnOkWithObject()
        {
            // Arrange
            mockPointsListService.Setup(s => s.Get(1)).Returns(mockedPointsListEmpty);
            var pointsListController = this.CreatePointsListController();
            int id = 1;

            // Act
            var result = pointsListController.Get(id);
            var okResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(mockedPointsListEmpty, okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);

        }
        [Test]
        public void Get_NotExistingPointList_ReturnNotFoundError()
        {
            // Arrange
            mockPointsListService.Setup(s => s.Get(2)).Returns((PointsList)null);
            var pointsListController = this.CreatePointsListController();
            

            // Act
            var result = pointsListController.Get(2);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.AreEqual(404, notFoundResult.StatusCode);

        }

        [Test]
        public void GetAll_TwoListsExist_ReturnAllExistingLists()
        {
            // Arrange
            mockPointsListService.Setup(s => s.GetAll()).Returns(new List<PointsList> { mockedPointsListEmpty, mockedPointsListOneSquare });
            var pointsListController = this.CreatePointsListController();

            // Act
            var result = pointsListController.GetAll();
            var okResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(new List<PointsList> { mockedPointsListEmpty, mockedPointsListOneSquare }, (List<PointsList>)okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);
        }
      
        [Test]
        public void Create_ValidModel_ReturnsCreatedList()
        {
            // Arrange
            mockPointsListService.Setup(s => s.Create(validCreatePointsListRequest)).Returns(mockedPointsListOneSquare);
            var pointsListController = this.CreatePointsListController();

            // Act
            var result = pointsListController.Create(validCreatePointsListRequest);
            var okResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(mockedPointsListOneSquare, okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);
        }
       
        [Test]
        public void GetSquares_PointsListExists_ReturnPointsListSquares()
        {
            // Arrange
            mockPointsListService.Setup(s => s.GetSquares(2)).Returns(mockedPointsListOneSquare.Squares);
            var pointsListController = this.CreatePointsListController();
            int pointsListId = 2;

            // Act
            var result = pointsListController.GetSquares(pointsListId);
            var okResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(mockedPointsListOneSquare.Squares, okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);

        }
        [Test]
        public void GetSquares_PointsListDoesNotExist_ReturnPointsListSquares()
        {
            // Arrange
            mockPointsListService.Setup(s => s.GetSquares(0)).Throws(new NotFoundException("Points list does not exist"));
            var pointsListController = this.CreatePointsListController();
            int pointsListId = 0;

            // Act
            var result = pointsListController.GetSquares(pointsListId);
            var badRequestResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(404, badRequestResult.StatusCode);

        }
        [Test]
        public void RemovePoint_PointExists_ReturnOk()
        {
            // Arrange
            mockPointsListService.Setup(s => s.RemovePoint(validRemovePointFromListRequest)).Returns(mockedPointsListOneSquare);
            var pointsListController = this.CreatePointsListController();

            // Act
            var result = pointsListController.RemovePoint(validRemovePointFromListRequest);
            var objectResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(mockedPointsListOneSquare, objectResult.Value);
            Assert.AreEqual(200, objectResult.StatusCode);
        }
        public void RemovePoint_PointsListDoesNotExist_ReturnBadRequest()
        {
            // Arrange
            mockPointsListService.Setup(s => s.RemovePoint(noListRemovePointFromListRequest)).Throws(new Exception("Points list does not exist"));
            var pointsListController = this.CreatePointsListController();

            // Act
            var result = pointsListController.RemovePoint(noListRemovePointFromListRequest);
            var objectResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(400, objectResult.StatusCode);
        }
        public void RemovePoint_PointDoesNotExist_ReturnBadRequest()
        {
            // Arrange
            mockPointsListService.Setup(s => s.RemovePoint(noPointRemovePointFromListRequest)).Throws(new Exception("Point does not exist in the list"));
            var pointsListController = this.CreatePointsListController();

            // Act
            var result = pointsListController.RemovePoint(noPointRemovePointFromListRequest);
            var objectReult = result as ObjectResult;

            // Assert
            Assert.AreEqual(400, objectReult.StatusCode);
        }

        [Test]
        public void Delete_ListExists_ReturnOk()
        {
            // Arrange
            mockPointsListService.Setup(s => s.DeletePointsList(1));
            var pointsListController = this.CreatePointsListController();

            // Act
            var result = pointsListController.Delete(1);
            var objectResult = result as OkResult;

            // Assert
            Assert.AreEqual(200, objectResult.StatusCode);
        }
        [Test]
        public void Delete_ListDoesNotExist_ReturnBadRequest()
        {
            // Arrange
            mockPointsListService.Setup(s => s.DeletePointsList(0)).Throws(new NotFoundException("Points list does not exist"));
            var pointsListController = this.CreatePointsListController();
            int pointsListId = 0;

            // Act
            var result = pointsListController.Delete(pointsListId);
            var objectResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(404, objectResult.StatusCode);

        }
        [Test]
        public void Addpoint_PointsListExists_ReturnOk()
        {
            // Arrange
            mockPointsListService.Setup(s => s.AddPoint(validAddPointToListRequest)).Returns(mockedPointsListOneSquare);
            var pointsListController = this.CreatePointsListController();

            // Act
            var result = pointsListController.AddPoint(validAddPointToListRequest);
            var objectResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(200, objectResult.StatusCode);
        }
        [Test]
        public void AddPoint_PointsListDoesNotExist_ReturnBadRequest()
        {
            // Arrange
            mockPointsListService.Setup(s => s.AddPoint(listDoesNotExistAddPointToListRequest)).Throws(new NotFoundException("Points list does not exist"));

            var pointsListController = this.CreatePointsListController();

            // Act
            var result = pointsListController.AddPoint(listDoesNotExistAddPointToListRequest);

            var objectResult = result as ObjectResult;

            // Assert
            Assert.AreEqual(404, objectResult.StatusCode);
        }
    }
}

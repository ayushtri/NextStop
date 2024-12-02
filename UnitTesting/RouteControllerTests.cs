using Moq;
using NUnit.Framework;
using NextStopApp.Controllers;
using NextStopApp.DTOs;
using NextStopApp.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace UnitTesting
{
    [TestFixture]
    public class RouteControllerTests
    {
        private Mock<IRouteService> _mockRouteService;
        private RouteController _routeController;

        [SetUp]
        public void Setup()
        {
            _mockRouteService = new Mock<IRouteService>();
            _routeController = new RouteController(_mockRouteService.Object);
        }

        [Test]
        public async Task AddRoute_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var routeDto = new RouteCreateDTO { /* ... */ };
            var createdRoute = new RouteDTO { /* ... */ };
            _mockRouteService.Setup(s => s.AddRoute(routeDto)).ReturnsAsync(createdRoute);

            // Act
            var result = await _routeController.AddRoute(routeDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as RouteResponseDTO; // Cast to specific type
            Assert.AreEqual("Route added successfully", response.Message);
            Assert.AreEqual(createdRoute, response.Route);
        }

        [Test]
        public async Task AddRoute_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            _routeController.ModelState.AddModelError("Key", "Error message");

            // Act
            var result = await _routeController.AddRoute(new RouteCreateDTO());

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
        }

        [Test]
        public async Task UpdateRoute_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var routeId = 1;
            var routeDto = new RouteUpdateDTO { /* ... */ };
            var updatedRoute = new RouteDTO { /* ... */ };
            _mockRouteService.Setup(s => s.UpdateRoute(routeId, routeDto)).ReturnsAsync(updatedRoute);

            // Act
            var result = await _routeController.UpdateRoute(routeId, routeDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var response = okResult.Value as RouteResponseDTO;
            Assert.AreEqual("Route updated successfully", response.Message);
            Assert.AreEqual(updatedRoute, response.Route);
        }

        [Test]
        public async Task UpdateRoute_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            _routeController.ModelState.AddModelError("Key", "Error message");

            // Act
            var result = await _routeController.UpdateRoute(1, new RouteUpdateDTO());

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
        }

        [Test]
        public async Task DeleteRoute_RouteExists_ReturnsOkResult()
        {
            // Arrange
            var routeId = 1;
            _mockRouteService.Setup(s => s.DeleteRoute(routeId)).ReturnsAsync(true);

            // Act
            var result = await _routeController.DeleteRoute(routeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Route deleted successfully.", okResult.Value);
        }

        [Test]
        public async Task DeleteRoute_RouteDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var routeId = 1;
            _mockRouteService.Setup(s => s.DeleteRoute(routeId)).ReturnsAsync(false);

            // Act
            var result = await _routeController.DeleteRoute(routeId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Route not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetAllRoutes_ReturnsOkResult_WithListOfRoutes()
        {
            // Arrange
            var routes = new List<RouteDTO> { /* ... */ };
            _mockRouteService.Setup(s => s.GetAllRoutes()).ReturnsAsync(routes);

            // Act
            var result = await _routeController.GetAllRoutes();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(routes, okResult.Value);
        }
    }
}
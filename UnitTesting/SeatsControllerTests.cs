using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NextStopApp.Controllers;
using NextStopApp.DTOs;
using NextStopApp.Repositories;
using NUnit.Framework;

namespace UnitTesting
{
    [TestFixture]
    public class SeatsControllerTests
    {
        private Mock<ISeatsService> _seatsServiceMock;
        private SeatsController _controller;

        [SetUp]
        public void SetUp()
        {
            _seatsServiceMock = new Mock<ISeatsService>();

            // Mock user context
            var httpContext = new DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "admin")
            }));

            _controller = new SeatsController(_seatsServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test]
        public async Task AddSeats_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var addSeatsDto = new AddSeatsDTO { BusId = 1, SeatNumbers = new List<string> { "A1", "A2" } };
            _seatsServiceMock.Setup(ss => ss.AddSeats(addSeatsDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddSeats(addSeatsDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Seats added successfully.", okResult.Value);
        }

        [Test]
        public async Task AddSeats_ServiceReturnsFalse_ReturnsBadRequest()
        {
            // Arrange
            var addSeatsDto = new AddSeatsDTO { BusId = 1, SeatNumbers = new List<string> { "A1", "A2" } };
            _seatsServiceMock.Setup(ss => ss.AddSeats(addSeatsDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddSeats(addSeatsDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Failed to add seats.", badRequestResult.Value);
        }

        [Test]
        public async Task GetAvailableSeats_ValidScheduleId_ReturnsOkResult()
        {
            // Arrange
            var scheduleId = 1;
            var availableSeats = new List<string> { "A1", "A2", "A3" };
            _seatsServiceMock.Setup(ss => ss.GetAvailableSeats(scheduleId)).ReturnsAsync(availableSeats);

            // Act
            var result = await _controller.GetAvailableSeats(scheduleId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(availableSeats, okResult.Value);
        }

        [Test]
        public async Task ReserveSeats_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var reserveSeatsDto = new ReserveSeatsDTO { ScheduleId = 1, SeatNumbers = new List<string> { "A1", "A2" } };
            _seatsServiceMock.Setup(ss => ss.ReserveSeats(reserveSeatsDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.ReserveSeats(reserveSeatsDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Seats reserved successfully.", okResult.Value);
        }

        [Test]
        public async Task ReserveSeats_ServiceReturnsFalse_ReturnsBadRequest()
        {
            // Arrange
            var reserveSeatsDto = new ReserveSeatsDTO { ScheduleId = 1, SeatNumbers = new List<string> { "A1", "A2" } };
            _seatsServiceMock.Setup(ss => ss.ReserveSeats(reserveSeatsDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.ReserveSeats(reserveSeatsDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Failed to reserve seats.", badRequestResult.Value);
        }

        [Test]
        public async Task ReleaseSeats_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var releaseSeatsDto = new ReleaseSeatsDTO { ScheduleId = 1, SeatNumbers = new List<string> { "A1", "A2" } };
            _seatsServiceMock.Setup(ss => ss.ReleaseSeats(releaseSeatsDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.ReleaseSeats(releaseSeatsDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Seats released successfully.", okResult.Value);
        }

        [Test]
        public async Task ReleaseSeats_ServiceReturnsFalse_ReturnsBadRequest()
        {
            // Arrange
            var releaseSeatsDto = new ReleaseSeatsDTO { ScheduleId = 1, SeatNumbers = new List<string> { "A1", "A2" } };
            _seatsServiceMock.Setup(ss => ss.ReleaseSeats(releaseSeatsDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.ReleaseSeats(releaseSeatsDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Failed to release seats.", badRequestResult.Value);
        }
    }
}

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
    public class SchedulesControllerTests
    {
        private Mock<ISchedulesService> _schedulesServiceMock;
        private SchedulesController _controller;

        [SetUp]
        public void SetUp()
        {
            _schedulesServiceMock = new Mock<ISchedulesService>();

            // Mock user context
            var httpContext = new DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "admin")
            }));

            _controller = new SchedulesController(_schedulesServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test]
        public async Task AddSchedule_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var scheduleDto = new ScheduleCreateDTO { BusId = 1, RouteId = 2, Date = DateTime.Now, Fare = 500 };
            var createdSchedule = new ScheduleDTO { ScheduleId = 1, BusId = 1, RouteId = 2, Date = DateTime.Now, Fare = 500 };

            _schedulesServiceMock.Setup(ss => ss.AddSchedule(scheduleDto)).ReturnsAsync(createdSchedule);

            // Act
            var result = await _controller.AddSchedule(scheduleDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

            Assert.NotNull(okResult);

            // Access properties of the anonymous object dynamically
            dynamic response = okResult.Value;
            Assert.AreEqual("Schedule added successfully", response.Message);
            Assert.AreEqual(createdSchedule, response.Schedule);
        }




        [Test]
        public async Task UpdateSchedule_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var scheduleDto = new ScheduleUpdateDTO { Date = DateTime.Now.AddDays(1), Fare = 600 };
            var updatedSchedule = new ScheduleDTO { ScheduleId = 1, BusId = 1, RouteId = 2, Date = DateTime.Now.AddDays(1), Fare = 600 };

            _schedulesServiceMock.Setup(ss => ss.UpdateSchedule(It.IsAny<int>(), scheduleDto)).ReturnsAsync(updatedSchedule);

            // Act
            var result = await _controller.UpdateSchedule(1, scheduleDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Schedule updated successfully", ((dynamic)okResult.Value).Message);
            Assert.AreEqual(updatedSchedule, ((dynamic)okResult.Value).Schedule);
        }

        [Test]
        public async Task DeleteSchedule_ScheduleExists_ReturnsOkResult()
        {
            // Arrange
            _schedulesServiceMock.Setup(ss => ss.DeleteSchedule(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteSchedule(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Schedule deleted successfully.", okResult.Value);
        }

        [Test]
        public async Task DeleteSchedule_ScheduleDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _schedulesServiceMock.Setup(ss => ss.DeleteSchedule(It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteSchedule(1);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Schedule not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetSchedules_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var schedules = new List<ScheduleDTO>
            {
                new ScheduleDTO { ScheduleId = 1, BusId = 1, RouteId = 2, Date = DateTime.Now, Fare = 500 },
                new ScheduleDTO { ScheduleId = 2, BusId = 1, RouteId = 3, Date = DateTime.Now.AddDays(1), Fare = 600 }
            };

            _schedulesServiceMock.Setup(ss => ss.GetSchedules(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>())).ReturnsAsync(schedules);

            // Act
            var result = await _controller.GetSchedules(null, null, null);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(schedules, okResult.Value);
        }
    }
}

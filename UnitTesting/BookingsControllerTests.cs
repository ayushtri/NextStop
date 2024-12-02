using Moq;
using NUnit.Framework;
using NextStopApp.Controllers;
using NextStopApp.DTOs;
using NextStopApp.Repositories; 
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestFixture]
    public class BookingsControllerTests
    {
        private Mock<IBookingService> _mockBookingService;
        private BookingsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockBookingService = new Mock<IBookingService>();
            _controller = new BookingsController(_mockBookingService.Object);
        }

        [Test]
        public async Task SearchBus_ShouldReturnOk_WhenSchedulesFound()
        {
            // Arrange
            var searchBusDto = new SearchBusDTO
            {
                Origin = "City A",
                Destination = "City B",
                TravelDate = DateTime.Now.AddDays(1)
            };

            var schedules = new List<ScheduleDTO>
            {
                new ScheduleDTO
                {
                    ScheduleId = 1,
                    BusId = 101,
                    RouteId = 1,
                    DepartureTime = DateTime.Now.AddHours(1),
                    ArrivalTime = DateTime.Now.AddHours(2),
                    Fare = 100.0m,
                    Date = DateTime.Now.AddDays(1)
                },
                new ScheduleDTO
                {
                    ScheduleId = 2,
                    BusId = 102,
                    RouteId = 2,
                    DepartureTime = DateTime.Now.AddHours(2),
                    ArrivalTime = DateTime.Now.AddHours(3),
                    Fare = 120.0m,
                    Date = DateTime.Now.AddDays(1)
                }
            };

            // Mocking SearchBus to return a list of ScheduleDTOs
            _mockBookingService.Setup(x => x.SearchBus(searchBusDto)).ReturnsAsync(schedules);

            // Act
            var result = await _controller.SearchBus(searchBusDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(schedules, okResult.Value);
        }

        [Test]
        public async Task SearchBus_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var searchBusDto = new SearchBusDTO
            {
                Origin = "City A",
                Destination = "City B",
                TravelDate = DateTime.Now.AddDays(1)
            };

            _mockBookingService.Setup(x => x.SearchBus(searchBusDto)).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.SearchBus(searchBusDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Error", badRequestResult.Value);
        }

        [Test]
        public async Task BookTicket_ShouldReturnOk_WhenTicketBooked()
        {
            // Arrange
            var bookTicketDto = new BookTicketDTO
            {
                UserId = 1,
                ScheduleId = 101,
                SelectedSeats = new List<string> { "A1", "A2" }
            };

            var bookingResponse = new BookingDTO
            {
                BookingId = 123,
                Status = "Confirmed"
            };

            // Mock the BookTicket method to return the BookingDTO
            _mockBookingService.Setup(x => x.BookTicket(bookTicketDto)).ReturnsAsync(bookingResponse);

            // Act
            var result = await _controller.BookTicket(bookTicketDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(bookingResponse, okResult.Value);
        }

        [Test]
        public async Task BookTicket_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var bookTicketDto = new BookTicketDTO
            {
                UserId = 1,
                ScheduleId = 101,
                SelectedSeats = new List<string> { "A1", "A2" }
            };

            _mockBookingService.Setup(x => x.BookTicket(bookTicketDto)).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.BookTicket(bookTicketDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Error", badRequestResult.Value);
        }

        [Test]
        public async Task CancelBooking_ShouldReturnOk_WhenBookingCancelled()
        {
            // Arrange
            var bookingId = 123;
            _mockBookingService.Setup(x => x.CancelBooking(bookingId)).ReturnsAsync(true);

            // Act
            var result = await _controller.CancelBooking(bookingId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Booking cancelled successfully.", okResult.Value);
        }

        [Test]
        public async Task CancelBooking_ShouldReturnNotFound_WhenBookingNotFound()
        {
            // Arrange
            var bookingId = 123;
            _mockBookingService.Setup(x => x.CancelBooking(bookingId)).ReturnsAsync(false);

            // Act
            var result = await _controller.CancelBooking(bookingId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Booking not found.", notFoundResult.Value);
        }

        [Test]
        public async Task ViewBookings_ShouldReturnOk_WhenBookingsFound()
        {
            // Arrange
            var userId = 1;
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, Status = "Confirmed" },
                new BookingDTO { BookingId = 2, Status = "Confirmed" }
            };

            _mockBookingService.Setup(x => x.ViewBookings(userId)).ReturnsAsync(bookings);

            // Act
            var result = await _controller.ViewBookings(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(bookings, okResult.Value);
        }

        [Test]
        public async Task ViewBookings_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var userId = 1;
            _mockBookingService.Setup(x => x.ViewBookings(userId)).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.ViewBookings(userId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Error", badRequestResult.Value);
        }
    }
}

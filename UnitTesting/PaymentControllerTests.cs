using Moq;
using NUnit.Framework;
using NextStopApp.Controllers;
using NextStopApp.DTOs;
using NextStopApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private PaymentController _controller;

        [SetUp]
        public void SetUp()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _controller = new PaymentController(_paymentServiceMock.Object);
        }

        [Test]
        public async Task InitiatePayment_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var initiatePaymentDto = new InitiatePaymentDTO
            {
                BookingId = 1,
                Amount = 100.0M,
                PaymentStatus = "successful"
            };

            var paymentStatus = new PaymentStatusDTO
            {
                PaymentId = 1,
                BookingId = 1,
                Amount = 100.0M,
                PaymentStatus = "successful",
                PaymentDate = DateTime.Now
            };

            _paymentServiceMock.Setup(ps => ps.InitiatePayment(It.IsAny<InitiatePaymentDTO>()))
                .ReturnsAsync(paymentStatus); // Return mocked PaymentStatusDTO

            // Act
            var result = await _controller.InitiatePayment(initiatePaymentDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(paymentStatus, okResult.Value);
        }

        [Test]
        public async Task ViewPaymentStatus_ValidBookingId_ReturnsOkResult()
        {
            // Arrange
            int bookingId = 1;
            var paymentStatus = new PaymentStatusDTO
            {
                PaymentId = 1,
                BookingId = 1,
                Amount = 100.0M,
                PaymentStatus = "successful",
                PaymentDate = DateTime.Now
            };

            _paymentServiceMock.Setup(ps => ps.GetPaymentStatus(It.IsAny<int>()))
                .ReturnsAsync(paymentStatus); // Mock payment status response

            // Act
            var result = await _controller.ViewPaymentStatus(bookingId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(paymentStatus, okResult.Value);
        }
    }
}

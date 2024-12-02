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
    public class NotificationsControllerTests
    {
        private Mock<INotificationService> _mockNotificationService;
        private NotificationsController _controller;

        [SetUp]
        public void Setup()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _controller = new NotificationsController(_mockNotificationService.Object);
        }

        [Test]
        public async Task SendNotification_ShouldReturnOk_WhenNotificationSentSuccessfully()
        {
            // Arrange
            var sendNotificationDto = new SendNotificationDTO
            {
                UserId = 1,
                Message = "Test Notification",
                NotificationType = "Info"
            };
            _mockNotificationService.Setup(x => x.SendNotification(It.IsAny<SendNotificationDTO>())).ReturnsAsync(true);

            // Act
            var result = await _controller.SendNotification(sendNotificationDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Notification sent successfully.", okResult.Value);
        }

        [Test]
        public async Task SendNotification_ShouldReturnBadRequest_WhenNotificationFailedToSend()
        {
            // Arrange
            var sendNotificationDto = new SendNotificationDTO
            {
                UserId = 1,
                Message = "Test Notification",
                NotificationType = "Error"
            };
            _mockNotificationService.Setup(x => x.SendNotification(It.IsAny<SendNotificationDTO>())).ReturnsAsync(false);

            // Act
            var result = await _controller.SendNotification(sendNotificationDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Failed to send notification.", badRequestResult.Value);
        }

        [Test]
        public async Task SendNotification_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var sendNotificationDto = new SendNotificationDTO
            {
                UserId = 1,
                Message = "Test Notification",
                NotificationType = "Error"
            };
            _mockNotificationService.Setup(x => x.SendNotification(It.IsAny<SendNotificationDTO>())).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.SendNotification(sendNotificationDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Something went wrong", badRequestResult.Value);
        }

        [Test]
        public async Task ViewNotifications_ShouldReturnOk_WhenNotificationsFound()
        {
            // Arrange
            int userId = 1;
            var notifications = new List<NotificationDTO>
            {
                new NotificationDTO { NotificationId = 1, Message = "Test Message 1", SentDate = DateTime.UtcNow, NotificationType = "Info" },
                new NotificationDTO { NotificationId = 2, Message = "Test Message 2", SentDate = DateTime.UtcNow, NotificationType = "Warning" }
            };

            // Mock the ViewNotifications method to return the List<NotificationDTO>
            _mockNotificationService.Setup(x => x.ViewNotifications(userId))
                                    .ReturnsAsync(notifications);

            // Act
            var result = await _controller.ViewNotifications(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(notifications, okResult.Value);
        }

        [Test]
        public async Task ViewNotifications_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            int userId = 1;
            _mockNotificationService.Setup(x => x.ViewNotifications(userId)).ThrowsAsync(new Exception("Error retrieving notifications"));

            // Act
            var result = await _controller.ViewNotifications(userId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Error retrieving notifications", badRequestResult.Value);
        }
    }
}

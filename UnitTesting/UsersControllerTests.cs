using System;
using System.Collections.Generic;
using System.Security.Claims;
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
    public class UsersControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, "admin@example.com"),
                new Claim(ClaimTypes.Role, "admin")
            }));

            _controller = new UsersController(_userServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test]
        public async Task GetAllUsers_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Name = "User1", Email = "user1@example.com" },
                new UserDTO { UserId = 2, Name = "User2", Email = "user2@example.com" }
            };

            _userServiceMock.Setup(us => us.GetAllUsers()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(users, okResult.Value);
        }

        [Test]
        public async Task GetUserById_UserExists_ReturnsOkResult()
        {
            // Arrange
            var user = new UserDTO { UserId = 1, Name = "User1", Email = "user1@example.com" };

            _userServiceMock.Setup(us => us.GetUserById(It.IsAny<int>())).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(user, okResult.Value);
        }

        [Test]
        public async Task GetUserById_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(us => us.GetUserById(It.IsAny<int>())).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User not found.", notFoundResult.Value);
        }

        [Test]
        public async Task UpdateUser_ValidData_ReturnsOkResult()
        {
            // Arrange
            var updateUserDto = new UpdateUserDTO { Name = "UpdatedUser" };
            var updatedUser = new UserDTO { UserId = 1, Name = "UpdatedUser", Email = "user1@example.com" };

            _userServiceMock.Setup(us => us.GetUserById(It.IsAny<int>())).ReturnsAsync(updatedUser);
            _userServiceMock.Setup(us => us.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserDTO>())).ReturnsAsync(updatedUser);

            // Act
            var result = await _controller.UpdateUser(1, updateUserDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(updatedUser, okResult.Value);
        }

        [Test]
        public async Task DeleteUser_UserExists_ReturnsOkResult()
        {
            // Arrange
            var user = new UserDTO { UserId = 1, Name = "User1", Email = "user1@example.com" };

            _userServiceMock.Setup(us => us.GetUserById(It.IsAny<int>())).ReturnsAsync(user);

            _userServiceMock.Setup(us => us.DeleteUser(It.IsAny<int>()))
                .ReturnsAsync(new UserDTO { UserId = 1, Name = "User1", Email = "user1@example.com" });

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("User has been deactivated successfully.", okResult.Value);
        }


        [Test]
        public async Task DeleteUser_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(us => us.GetUserById(It.IsAny<int>())).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User not found.", notFoundResult.Value);
        }
    }
}

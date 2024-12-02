using Moq;
using NUnit.Framework;
using NextStopApp.Controllers;
using NextStopApp.DTOs;
using NextStopApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextStopApp.Models;

namespace UnitTesting
{
    [TestFixture]
    public class BusControllerTests
    {
        private Mock<IBusService> _mockBusService;
        private BusController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBusService = new Mock<IBusService>();
            _controller = new BusController(_mockBusService.Object);
        }

        [Test]
        public async Task RegisterBusOperator_ShouldReturnOk_WhenOperatorRegistered()
        {
            // Arrange
            var operatorDto = new BusOperatorRegisterDTO
            {
                Name = "John Doe",
                ContactNumber = "1234567890",
                Email = "johndoe@example.com",
                Address = "123 Main St"
            };

            var createdOperator = new BusOperator
            {
                Name = "John Doe",
                ContactNumber = "1234567890",
                Email = "johndoe@example.com",
                Address = "123 Main St"
            };

            // Create the response DTO
            var responseDto = new BusOperatorResponseDTO
            {
                Message = "Bus operator registered successfully",
                Operator = new BusOperator
                {
                    Name = createdOperator.Name,
                    ContactNumber = createdOperator.ContactNumber,
                    Email = createdOperator.Email,
                    Address = createdOperator.Address
                }
            };

            // Adjust mock to return the response DTO, not just the operator
            _mockBusService.Setup(x => x.RegisterBusOperator(operatorDto)).ReturnsAsync(createdOperator);

            // Act
            var result = await _controller.RegisterBusOperator(operatorDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as BusOperatorResponseDTO;

            Assert.IsNotNull(response);
            Assert.AreEqual(responseDto.Message, response.Message);
            Assert.AreEqual(responseDto.Operator.Name, response.Operator.Name);
            Assert.AreEqual(responseDto.Operator.ContactNumber, response.Operator.ContactNumber);
            Assert.AreEqual(responseDto.Operator.Email, response.Operator.Email);
            Assert.AreEqual(responseDto.Operator.Address, response.Operator.Address);
        }



        [Test]
        public async Task RegisterBusOperator_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var operatorDto = new BusOperatorRegisterDTO();  // Invalid DTO (missing required fields)
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.RegisterBusOperator(operatorDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task AddBus_ShouldReturnOk_WhenBusAdded()
        {
            // Arrange
            var busDto = new BusCreateDTO
            {
                OperatorId = 1,
                BusName = "Bus 101",
                BusNumber = "1234",
                BusType = "City Bus",
                TotalSeats = 40
            };

            var createdBus = new BusDTO
            {
                OperatorId = 1,
                BusName = "Bus 101",
                BusNumber = "1234",
                BusType = "City Bus",
                TotalSeats = 40
            };

            // Create the response DTO
            var responseDto = new BusResponseDTO
            {
                Message = "Bus added successfully",
                Bus = createdBus
            };

            _mockBusService.Setup(x => x.AddBus(busDto)).ReturnsAsync(createdBus);

            // Act
            var result = await _controller.AddBus(busDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as BusResponseDTO;

            Assert.IsNotNull(response);
            Assert.AreEqual(responseDto.Message, response.Message);
            Assert.AreEqual(responseDto.Bus.OperatorId, response.Bus.OperatorId);
            Assert.AreEqual(responseDto.Bus.BusName, response.Bus.BusName);
            Assert.AreEqual(responseDto.Bus.BusNumber, response.Bus.BusNumber);
            Assert.AreEqual(responseDto.Bus.BusType, response.Bus.BusType);
            Assert.AreEqual(responseDto.Bus.TotalSeats, response.Bus.TotalSeats);
        }



        [Test]
        public async Task UpdateBus_ShouldReturnOk_WhenBusUpdated()
        {
            // Arrange
            var updateBusDto = new BusUpdateDTO
            {
                BusName = "Updated Bus",
                BusType = "Luxury Bus",
                TotalSeats = 50,
                Amenities = "Wi-Fi, AC"
            };

            _mockBusService.Setup(x => x.UpdateBus(It.IsAny<int>(), updateBusDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateBus(1, updateBusDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Bus updated successfully", okResult.Value);
        }

        [Test]
        public async Task DeleteBus_ShouldReturnOk_WhenBusDeleted()
        {
            // Arrange
            _mockBusService.Setup(x => x.DeleteBus(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBus(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Bus deleted successfully", okResult.Value);
        }

        [Test]
        public async Task ViewBuses_ShouldReturnOk_WhenBusesFound()
        {
            // Arrange
            var buses = new List<BusDTO>  
            {
                new BusDTO { BusName = "Bus 101", BusNumber = "1234", BusType = "City Bus", TotalSeats = 40 },
                new BusDTO { BusName = "Bus 102", BusNumber = "1235", BusType = "City Bus", TotalSeats = 50 }
            };

            _mockBusService.Setup(x => x.GetBusesByOperatorId(It.IsAny<int>())).ReturnsAsync(buses);

            // Act
            var result = await _controller.ViewBuses(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(buses, okResult.Value);
        }


        [Test]
        public async Task GetBusOperatorByEmail_ShouldReturnOk_WhenOperatorFound()
        {
            // Arrange
            var email = "johndoe@example.com";
            var busOperator = new BusOperator
            {
                Name = "John Doe",
                ContactNumber = "1234567890",
                Email = email,
                Address = "123 Main St"
            };

            // Mocking the method
            _mockBusService.Setup(x => x.GetBusOperatorByEmail(email)).ReturnsAsync(busOperator);

            // Act
            var result = await _controller.GetBusOperatorByEmail(email);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(busOperator, okResult.Value);
        }


        [Test]
        public async Task GetBusByBusNumber_ShouldReturnOk_WhenBusFound()
        {
            // Arrange
            var busNumber = "1234";
            var bus = new BusDTO  // Assuming the method expects BusDTO
            {
                OperatorId = 1,
                BusName = "Bus 101",
                BusNumber = busNumber,
                BusType = "City Bus",
                TotalSeats = 40
            };

            _mockBusService.Setup(x => x.GetBusByBusNumber(busNumber)).ReturnsAsync(bus);

            // Act
            var result = await _controller.GetBusByBusNumber(busNumber);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(bus, okResult.Value);
        }


        [Test]
        public async Task ViewAllOperators_ShouldReturnOk_WhenOperatorsFound()
        {
            // Arrange
            var operators = new List<BusOperatorDTO>  // Use BusOperatorDTO instead of BusOperatorRegisterDTO
            {
                new BusOperatorDTO { Name = "John Doe", ContactNumber = "1234567890", Email = "johndoe@example.com", Address = "123 Main St" },
                new BusOperatorDTO { Name = "Jane Doe", ContactNumber = "0987654321", Email = "janedoe@example.com", Address = "456 Elm St" }
            };

            _mockBusService.Setup(x => x.ViewAllOperators()).ReturnsAsync(operators);

            // Act
            var result = await _controller.ViewAllOperators();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(operators, okResult.Value);
        }

    }
}

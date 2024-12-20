﻿using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextStopApp.DTOs;
using NextStopApp.Repositories;

namespace NextStopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,operator")]
    public class BusController : ControllerBase
    {
        private readonly IBusService _busService;
        private static readonly ILog _log = LogManager.GetLogger(typeof(BusController));

        public BusController(IBusService busService)
        {
            _busService = busService;
        }

        [HttpPost("RegisterOperator")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RegisterBusOperator([FromBody] BusOperatorRegisterDTO operatorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdOperator = await _busService.RegisterBusOperator(operatorDto);

                // Create the response DTO
                var response = new BusOperatorResponseDTO
                {
                    Message = "Bus operator registered successfully",
                    Operator = createdOperator
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred while registering bus operator.", ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("AddBus")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> AddBus([FromBody] BusCreateDTO busDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdBus = await _busService.AddBus(busDto);
                return Ok(new BusResponseDTO
                {
                    Message = "Bus added successfully",
                    Bus = createdBus
                });
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred while adding a bus.", ex); 
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateBus/{busId}")]
        public async Task<IActionResult> UpdateBus(int busId, [FromBody] BusUpdateDTO updateBusDto)
        {
            try
            {
                await _busService.UpdateBus(busId, updateBusDto);
                return Ok("Bus updated successfully");
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while updating bus with ID {busId}.", ex); 
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteBus/{busId}")]
        public async Task<IActionResult> DeleteBus(int busId)
        {
            try
            {
                await _busService.DeleteBus(busId);
                return Ok("Bus deleted successfully");
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while deleting bus with ID {busId}.", ex);  
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ViewBuses/{operatorId}")]
        public async Task<IActionResult> ViewBuses(int operatorId)
        {
            try
            {
                var buses = await _busService.GetBusesByOperatorId(operatorId);
                return Ok(buses);
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while fetching buses for operator ID {operatorId}.", ex); 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetOperatorByEmail")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetBusOperatorByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            try
            {
                var busOperator = await _busService.GetBusOperatorByEmail(email);
                return Ok(busOperator);
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while fetching bus operator by email {email}.", ex);  
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetBusByNumber")]
        public async Task<IActionResult> GetBusByBusNumber([FromQuery] string busNumber)
        {
            if (string.IsNullOrEmpty(busNumber))
            {
                return BadRequest("Bus number is required.");
            }

            try
            {
                var bus = await _busService.GetBusByBusNumber(busNumber);
                return Ok(bus);
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while fetching bus with number {busNumber}.", ex);  
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ViewAllOperators")]
        public async Task<IActionResult> ViewAllOperators()
        {
            try
            {
                var operators = await _busService.ViewAllOperators();
                return Ok(operators);
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while viewing all operators.", ex);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

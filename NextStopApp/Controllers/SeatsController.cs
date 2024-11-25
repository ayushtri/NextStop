using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextStopApp.DTOs;
using NextStopApp.Repositories;

namespace NextStopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatsService _seatsService;

        public SeatsController(ISeatsService seatsService)
        {
            _seatsService = seatsService;
        }

        // Add Seats
        [HttpPost("AddSeats")]
        [Authorize(Roles = "admin,operator")]
        public async Task<IActionResult> AddSeats([FromBody] AddSeatsDTO addSeatsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _seatsService.AddSeats(addSeatsDto);
                if (result)
                    return Ok("Seats added successfully.");

                return BadRequest("Failed to add seats.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get Available Seats
        [HttpGet("{scheduleId}/AvailableSeats")]
        public async Task<IActionResult> GetAvailableSeats(int scheduleId)
        {
            try
            {
                var availableSeats = await _seatsService.GetAvailableSeats(scheduleId);
                return Ok(availableSeats);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Reserve Seats
        [HttpPost("ReserveSeats")]
        [Authorize(Roles = "admin,operator")]
        public async Task<IActionResult> ReserveSeats([FromBody] ReserveSeatsDTO reserveSeatsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _seatsService.ReserveSeats(reserveSeatsDto);
                if (result)
                    return Ok("Seats reserved successfully.");

                return BadRequest("Failed to reserve seats.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Release Seats
        [HttpPost("ReleaseSeats")]
        [Authorize(Roles = "admin,operator")]
        public async Task<IActionResult> ReleaseSeats([FromBody] ReleaseSeatsDTO releaseSeatsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _seatsService.ReleaseSeats(releaseSeatsDto);
                if (result)
                    return Ok("Seats released successfully.");

                return BadRequest("Failed to release seats.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
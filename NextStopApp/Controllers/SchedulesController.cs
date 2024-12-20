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
    [Authorize(Roles = "admin")]
    public class SchedulesController : ControllerBase
    {
        private readonly ISchedulesService _schedulesService;
        private static readonly ILog _log = LogManager.GetLogger(typeof(SchedulesController));

        public SchedulesController(ISchedulesService schedulesService)
        {
            _schedulesService = schedulesService;
        }

        [HttpPost("AddSchedule")]
        public async Task<IActionResult> AddSchedule([FromBody] ScheduleCreateDTO scheduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSchedule = await _schedulesService.AddSchedule(scheduleDto);
                return Ok(new AddScheduleResponseDTO
                {
                    Message = "Schedule added successfully",
                    Schedule = createdSchedule
                });
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred while adding schedule.", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateSchedule/{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule(int scheduleId, [FromBody] ScheduleUpdateDTO scheduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedSchedule = await _schedulesService.UpdateSchedule(scheduleId, scheduleDto);
                return Ok(new UpdateScheduleResponseDTO
                {
                    Message = "Schedule updated successfully",
                    Schedule = updatedSchedule
                });
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while updating schedule with ID {scheduleId}.", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteSchedule/{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(int scheduleId)
        {
            try
            {
                var isDeleted = await _schedulesService.DeleteSchedule(scheduleId);
                if (!isDeleted)
                {
                    return NotFound("Schedule not found.");
                }

                return Ok("Schedule deleted successfully.");
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while deleting schedule with ID {scheduleId}.", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetSchedules")]
        public async Task<IActionResult> GetSchedules([FromQuery] int? busId, [FromQuery] int? routeId, [FromQuery] DateTime? date)
        {
            try
            {
                var schedules = await _schedulesService.GetSchedules(busId, routeId, date);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred while fetching schedules.", ex);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

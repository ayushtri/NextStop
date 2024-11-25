using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopApp.DTOs;
using NextStopApp.Repositories;

namespace NextStopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        // 8.1 View All Users API
        [HttpGet("ViewAllUsers")]
        public async Task<IActionResult> ViewAllUsers()
        {
            try
            {
                var users = await _adminDashboardService.ViewAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 8.2 Assign Role API
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO assignRoleDto)
        {
            try
            {
                var result = await _adminDashboardService.AssignRole(assignRoleDto);
                if (result)
                {
                    return Ok("Role assigned successfully.");
                }
                return BadRequest("Failed to assign role.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 8.3 Generate Reports API
        [HttpPost("GenerateReports")]
        public async Task<IActionResult> GenerateReports([FromBody] GenerateReportsDTO generateReportsDto)
        {
            try
            {
                var report = await _adminDashboardService.GenerateReports(generateReportsDto);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

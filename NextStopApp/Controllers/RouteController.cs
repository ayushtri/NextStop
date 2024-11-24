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
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpPost("AddRoute")]
        public async Task<IActionResult> AddRoute([FromBody] RouteCreateDTO routeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdRoute = await _routeService.AddRoute(routeDto);
                return Ok(new
                {
                    Message = "Route added successfully",
                    Route = createdRoute
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateRoute/{routeId}")]
        public async Task<IActionResult> UpdateRoute(int routeId, [FromBody] RouteUpdateDTO routeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRoute = await _routeService.UpdateRoute(routeId, routeDto);
                return Ok(new
                {
                    Message = "Route updated successfully",
                    Route = updatedRoute
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteRoute/{routeId}")]
        public async Task<IActionResult> DeleteRoute(int routeId)
        {
            try
            {
                var isDeleted = await _routeService.DeleteRoute(routeId);
                if (!isDeleted)
                {
                    return NotFound("Route not found.");
                }

                return Ok("Route deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllRoutes")]
        public async Task<IActionResult> GetAllRoutes()
        {
            try
            {
                var routes = await _routeService.GetAllRoutes();
                return Ok(routes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

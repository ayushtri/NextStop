using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextStopApp.Repositories;

namespace NextStopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);

        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin-only")]
        public IActionResult GetAdminOnlyData()
        {
            return Ok("This data is only accessible to admins");
        }

        [Authorize(Roles = "admin,operator")]
        [HttpGet("operator-or-admin")]
        public IActionResult GetOperatorOrAdminData()
        {
            return Ok("This data is accessible to admins and operators");
        }

        [Authorize(Roles = "admin,passenger")]
        [HttpGet("passenger-or-admin")]
        public IActionResult GetPassengerOrAdminData()
        {
            return Ok("This data is accessible to admins and passengers");
        }
    }
}

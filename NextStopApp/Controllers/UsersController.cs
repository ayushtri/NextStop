using log4net;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private static readonly ILog _log = LogManager.GetLogger(typeof(UsersController));

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception
                _log.Error("Error occurred while retrieving all users.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log the exception
                _log.Error($"Error occurred while retrieving user with ID {userId}.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDTO updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid request body.");
                }

                var userToUpdate = await _userService.GetUserById(userId);
                if (userToUpdate == null)
                {
                    return NotFound("User not found.");
                }

                var currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                if (userToUpdate.Email != currentUserEmail && !User.IsInRole("admin"))
                {
                    return Forbid("You are not authorized to update this user's information.");
                }

                var updatedUser = await _userService.UpdateUser(userId, updateUserDto);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                // Log the exception
                _log.Error($"Error occurred while updating user with ID {userId}.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{userId}/reset-email")]
        public async Task<IActionResult> ResetEmail(int userId, [FromBody] string newEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(newEmail))
                {
                    return BadRequest("Email cannot be empty.");
                }

                var user = await _userService.GetUserById(userId);
                if (user == null || !user.IsActive)
                {
                    return NotFound("User not found or is deactivated.");
                }

                var currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                if (user.Email != currentUserEmail && !User.IsInRole("admin"))
                {
                    return Forbid("You are not authorized to reset this user's email.");
                }

                await _userService.ResetEmail(userId, newEmail);
                return Ok("Email has been reset successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                _log.Error($"Error occurred while resetting email for user with ID {userId}.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{userId}/reset-password")]
        public async Task<IActionResult> ResetPassword(int userId, [FromBody] string newPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(newPassword))
                {
                    return BadRequest("Password cannot be empty.");
                }

                var user = await _userService.GetUserById(userId);
                if (user == null || !user.IsActive)
                {
                    return NotFound("User not found or is deactivated.");
                }

                var currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                if (user.Email != currentUserEmail && !User.IsInRole("admin"))
                {
                    return Forbid("You are not authorized to reset this user's password.");
                }

                await _userService.ResetPassword(userId, newPassword);
                return Ok("Password has been reset successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                _log.Error($"Error occurred while resetting password for user with ID {userId}.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                if (user.Email != currentUserEmail && !User.IsInRole("admin"))
                {
                    return Forbid("You are not authorized to deactivate this user's account.");
                }

                await _userService.DeleteUser(userId);
                return Ok("User has been deactivated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                _log.Error($"Error occurred while deleting user with ID {userId}.", ex);
                return StatusCode(500, "Internal server error");
            }
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

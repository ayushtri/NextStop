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

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDTO updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request body.");
            }

            try
            {
                var userToUpdate = await _userService.GetUserById(userId);

                if (userToUpdate == null)
                {
                    return NotFound("User not found.");
                }

                //Check if the user is allowed to update
                var currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                // Admins can update any user; other roles can only update themselves
                if (userToUpdate.Email != currentUserEmail && !User.IsInRole("admin"))
                {
                    return Forbid("You are not authorized to update this user's information.");
                }

                var updatedUser = await _userService.UpdateUser(userId, updateUserDto);

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{userId}/reset-email")]
        public async Task<IActionResult> ResetEmail(int userId, [FromBody] string newEmail)
        {
            if (string.IsNullOrEmpty(newEmail))
            {
                return BadRequest("Email cannot be empty.");
            }

            try
            {
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("{userId}/reset-password")]
        public async Task<IActionResult> ResetPassword(int userId, [FromBody] string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Password cannot be empty.");
            }

            try
            {
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

                // Get the current user's email from the claims
                var currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                // Check if the current user is trying to delete their own account or if they are an admin
                if (user.Email != currentUserEmail && !User.IsInRole("admin"))
                {
                    return Forbid("You are not authorized to deactivate this user's account.");
                }

                // Deactivate the user (soft delete)
                await _userService.DeleteUser(userId);
                return Ok("User has been deactivated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

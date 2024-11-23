using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NextStopApp.DTOs;
using NextStopApp.Models;
using NextStopApp.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NextStopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IUserService userService, ITokenService tokenService, IConfiguration configuration)
        {
            _userService = userService;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request body.");
            }
            try
            {
                var user = await _userService.GetUserByEmailAndPassword(loginDto.Email, loginDto.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                if (!user.IsActive)
                {
                    return BadRequest("User account is deactivated");
                }

                var accessToken = IssueAccessToken(user);

                var refreshToken = GenerateRefreshToken();
                await _tokenService.SaveRefreshToken(user.Email, refreshToken);

                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO createUserDto)
        {
            if (string.IsNullOrEmpty(createUserDto.Role))
            {
                createUserDto.Role = "passenger";
            }

            if (string.IsNullOrEmpty(createUserDto.Email))
            {
                return BadRequest("Email is required.");
            }

            var existingUser = await _userService.GetUserByEmail(createUserDto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }

            try
            {
                var createdUser = await _userService.RegisterUser(createUserDto);

                var token = IssueAccessToken(createdUser);

                var userDto = new UserDTO
                {
                    UserId = createdUser.UserId,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    Phone = createdUser.Phone,
                    Address = createdUser.Address,
                    Role = createdUser.Role,
                    IsActive = createdUser.IsActive
                };

                return Ok(new { token = token, user = userDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Registration failed.");
            }
        }
        private string IssueAccessToken(UserDTO userDto)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.UserId.ToString()),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(ClaimTypes.Role, userDto.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            try
            {
                var email = await _tokenService.RetrieveEmailByRefreshToken(request.RefreshToken);

                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Invalid refresh token.");
                }

                var user = await _userService.GetUserByEmail(email);

                if (user == null)
                {
                    return Unauthorized("Invalid user.");
                }

                await _tokenService.RevokeRefreshToken(request.RefreshToken);

                var accessToken = IssueAccessToken(user);
                var newRefreshToken = GenerateRefreshToken();

                await _tokenService.SaveRefreshToken(email, newRefreshToken);

                return Ok(new { AccessToken = accessToken, RefreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            try
            {
                var isRevoked = await _tokenService.RevokeRefreshToken(request.RefreshToken);

                if (!isRevoked)
                {
                    return NotFound("Refresh token not found.");
                }

                return Ok("Token revoked successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            try
            {
                var email = await _tokenService.RetrieveEmailByRefreshToken(request.RefreshToken);

                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Invalid refresh token.");
                }

                var isRevoked = await _tokenService.RevokeRefreshToken(request.RefreshToken);

                if (!isRevoked)
                {
                    return NotFound("Refresh token not found or already revoked.");
                }

                return Ok("User logged out successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

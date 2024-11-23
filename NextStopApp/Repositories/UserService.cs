using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;

namespace NextStopApp.Repositories
{
    public class UserService : IUserService
    {
        NextStopDbContext _context;
        public UserService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users.Select(user => new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive
            }).ToList();
        }

        public async Task<UserDTO> GetUserById(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<UserDTO> RegisterUser(UserRegisterDTO createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                PasswordHash = HashPassword(createUserDto.PasswordHash),
                Phone = createUserDto.Phone,
                Address = createUserDto.Address,
                Role = createUserDto.Role,
                IsActive = true 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        //soft deletion
        public async Task<UserDTO> DeleteUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return null;

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            };
        }

        public async Task<UserDTO> ReactivateUser(int userId)
        {
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return null;

            user.IsActive = true;
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersIncludingDeactivated()
        {
            var users = await _context.Users.IgnoreQueryFilters().ToListAsync();

            return users.Select(user => new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive
            }).ToList();
        }


        public async Task<UserDTO> UpdateUser(int userId, UpdateUserDTO updateUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return null;

            user.Name = updateUserDto.Name ?? user.Name;
            user.Phone = updateUserDto.Phone ?? user.Phone;
            user.Address = updateUserDto.Address ?? user.Address;

            await _context.SaveChangesAsync();

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }


        public async Task<UserDTO> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<UserDTO> GetUserByEmailAndPassword(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.PasswordHash)) // Verify password against hashed value
                return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task ResetEmail(int userId, string newEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Ensure the email is unique
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newEmail);
            if (existingUser != null)
            {
                throw new Exception("The provided email already exists.");
            }

            user.Email = newEmail;

            await _context.SaveChangesAsync();
        }

        public async Task ResetPassword(int userId, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Hash the new password
            user.PasswordHash = HashPassword(newPassword);

            // Save the changes
            await _context.SaveChangesAsync();
        }


        private string HashPassword(string password)
        {
            // Hash the password using BCrypt (or any other secure algorithm)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            // Verify the plain password against the hashed password
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}

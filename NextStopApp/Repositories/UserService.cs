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
                Role = user.Role
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
                Role = user.Role
            };
        }

        public async Task<UserDTO> RegisterUser(UserRegisterDTO createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                PasswordHash = createUserDto.PasswordHash, 
                Phone = createUserDto.Phone,
                Address = createUserDto.Address,
                Role = createUserDto.Role
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
                Role = user.Role
            };
        }

        public async Task<UserDTO> DeleteUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return null;

            _context.Users.Remove(user);
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
                Role = user.Role
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
                Role = user.Role
            };
        }

        public async Task<UserDTO> GetUserByEmailAndPassword(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
            if (user == null)
                return null;

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
        private string HashPassword(string password)
        {
            return password; 
        }
    }
}

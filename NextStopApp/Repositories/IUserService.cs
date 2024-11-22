using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task<UserDTO> GetUserById(int userId);
        Task<UserDTO> RegisterUser(UserRegisterDTO createUserDto);
        Task<UserDTO> DeleteUser(int userId);
        Task<UserDTO> UpdateUser(int userId, UpdateUserDTO updateUserDto);
        Task<UserDTO> GetUserByEmail(string email);
        Task<UserDTO> GetUserByEmailAndPassword(string email, string password);
    }
}

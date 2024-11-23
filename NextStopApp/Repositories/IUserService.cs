using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task<UserDTO> GetUserById(int userId);
        Task<UserDTO> RegisterUser(UserRegisterDTO createUserDto);
        Task<UserDTO> DeleteUser(int userId);
        Task<UserDTO> ReactivateUser(int userId);
        Task<IEnumerable<UserDTO>> GetAllUsersIncludingDeactivated();
        Task<UserDTO> UpdateUser(int userId, UpdateUserDTO updateUserDto);
        Task<UserDTO> GetUserByEmail(string email);
        Task<UserDTO> GetUserByEmailAndPassword(string email, string password);
        Task ResetEmail(int userId, string newEmail);
        Task ResetPassword(int userId, string newPassword);
    }
}

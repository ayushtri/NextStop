using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface IAdminDashboardService
    {
        Task<IEnumerable<UserDTO>> ViewAllUsers();
        Task<bool> AssignRole(AssignRoleDTO assignRoleDto);
        Task<ReportDTO> GenerateReports(GenerateReportsDTO generateReportsDto);
    }
}

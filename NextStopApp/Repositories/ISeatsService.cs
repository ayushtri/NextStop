using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface ISeatsService
    {
        Task<List<string>> GetAvailableSeats(int scheduleId);
        Task<bool> ReserveSeats(ReserveSeatsDTO reserveSeatsDto);
        Task<bool> ReleaseSeats(ReleaseSeatsDTO releaseSeatsDto);
        Task<bool> AddSeats(AddSeatsDTO addSeatsDto);
    }
}

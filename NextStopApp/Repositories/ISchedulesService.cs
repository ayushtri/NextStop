using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface ISchedulesService
    {
        Task<ScheduleDTO> AddSchedule(ScheduleCreateDTO scheduleDto);
        Task<ScheduleDTO> UpdateSchedule(int scheduleId, ScheduleUpdateDTO scheduleDto);
        Task<bool> DeleteSchedule(int scheduleId);
        Task<IEnumerable<ScheduleDTO>> GetSchedules(int? busId, int? routeId, DateTime? date);
    }
}

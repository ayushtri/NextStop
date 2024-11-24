using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;

namespace NextStopApp.Repositories
{
    public class SchedulesService : ISchedulesService
    {
        private readonly NextStopDbContext _context;

        public SchedulesService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleDTO> AddSchedule(ScheduleCreateDTO scheduleDto)
        {
            var schedule = new Schedule
            {
                BusId = scheduleDto.BusId,
                RouteId = scheduleDto.RouteId,
                DepartureTime = scheduleDto.DepartureTime,
                ArrivalTime = scheduleDto.ArrivalTime,
                Fare = scheduleDto.Fare,
                Date = scheduleDto.Date
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return new ScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                BusId = schedule.BusId,
                RouteId = schedule.RouteId,
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                Fare = schedule.Fare,
                Date = schedule.Date
            };
        }

        public async Task<ScheduleDTO> UpdateSchedule(int scheduleId, ScheduleUpdateDTO scheduleDto)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found.");
            }

            schedule.DepartureTime = scheduleDto.DepartureTime ?? schedule.DepartureTime;
            schedule.ArrivalTime = scheduleDto.ArrivalTime ?? schedule.ArrivalTime;
            schedule.Fare = scheduleDto.Fare ?? schedule.Fare;
            schedule.Date = scheduleDto.Date ?? schedule.Date;

            await _context.SaveChangesAsync();

            return new ScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                BusId = schedule.BusId,
                RouteId = schedule.RouteId,
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                Fare = schedule.Fare,
                Date = schedule.Date
            };
        }

        public async Task<bool> DeleteSchedule(int scheduleId)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            if (schedule == null)
            {
                return false;
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ScheduleDTO>> GetSchedules(int? busId, int? routeId, DateTime? date)
        {
            var query = _context.Schedules.AsQueryable();

            if (busId.HasValue)
            {
                query = query.Where(s => s.BusId == busId.Value);
            }

            if (routeId.HasValue)
            {
                query = query.Where(s => s.RouteId == routeId.Value);
            }

            if (date.HasValue)
            {
                query = query.Where(s => s.Date.Date == date.Value.Date);
            }

            var schedules = await query.ToListAsync();

            return schedules.Select(schedule => new ScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                BusId = schedule.BusId,
                RouteId = schedule.RouteId,
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                Fare = schedule.Fare,
                Date = schedule.Date
            });
        }
    }
}

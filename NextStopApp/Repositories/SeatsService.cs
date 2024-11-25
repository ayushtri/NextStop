using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;

namespace NextStopApp.Repositories
{
    public class SeatsService : ISeatsService
    {
        private readonly NextStopDbContext _context;

        public SeatsService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetAvailableSeats(int scheduleId)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null)
                throw new Exception("Schedule not found.");

            var availableSeats = await _context.Seats
                .Where(seat => seat.BusId == schedule.BusId && seat.IsAvailable)
                .Select(seat => seat.SeatNumber)
                .ToListAsync();

            return availableSeats; // Return list of seat numbers as strings
        }

        public async Task<bool> ReserveSeats(ReserveSeatsDTO reserveSeatsDto)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.ScheduleId == reserveSeatsDto.ScheduleId);

            if (schedule == null)
                throw new Exception("Schedule not found.");

            var seatsToReserve = await _context.Seats
                .Where(seat => seat.BusId == schedule.BusId && reserveSeatsDto.SeatNumbers.Contains(seat.SeatNumber))
                .ToListAsync();

            if (seatsToReserve.Count != reserveSeatsDto.SeatNumbers.Count)
                throw new Exception("Some of the specified seats are not valid.");

            foreach (var seat in seatsToReserve)
            {
                if (!seat.IsAvailable)
                    throw new Exception($"Seat {seat.SeatNumber} is already reserved.");

                seat.IsAvailable = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseSeats(ReleaseSeatsDTO releaseSeatsDto)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.ScheduleId == releaseSeatsDto.ScheduleId);

            if (schedule == null)
                throw new Exception("Schedule not found.");

            var seatsToRelease = await _context.Seats
                .Where(seat => seat.BusId == schedule.BusId && releaseSeatsDto.SeatNumbers.Contains(seat.SeatNumber))
                .ToListAsync();

            if (seatsToRelease.Count != releaseSeatsDto.SeatNumbers.Count)
                throw new Exception("Some of the specified seats are not valid.");

            foreach (var seat in seatsToRelease)
            {
                if (seat.IsAvailable)
                    throw new Exception($"Seat {seat.SeatNumber} is already available.");

                seat.IsAvailable = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddSeats(AddSeatsDTO addSeatsDto)
        {
            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusId == addSeatsDto.BusId);
            if (bus == null)
                throw new Exception("Bus not found.");

            var existingSeats = await _context.Seats
                .Where(seat => seat.BusId == addSeatsDto.BusId && addSeatsDto.SeatNumbers.Contains(seat.SeatNumber))
                .ToListAsync();

            if (existingSeats.Any())
                throw new Exception("Some of the seat numbers already exist for this bus.");

            var seatsToAdd = addSeatsDto.SeatNumbers.Select(seatNumber => new Seat
            {
                BusId = addSeatsDto.BusId,
                SeatNumber = seatNumber,
                IsAvailable = true
            }).ToList();

            await _context.Seats.AddRangeAsync(seatsToAdd);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

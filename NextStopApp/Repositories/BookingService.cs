using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;

namespace NextStopApp.Repositories
{
    public class BookingService : IBookingService
    {
        private readonly NextStopDbContext _context;

        public BookingService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduleDTO>> SearchBus(SearchBusDTO searchBusDto)
        {
            var schedules = await _context.Schedules
                .Include(s => s.Route)
                .Where(s => s.Route.Origin == searchBusDto.Origin &&
                            s.Route.Destination == searchBusDto.Destination &&
                            s.Date.Date == searchBusDto.TravelDate.Date)
                .ToListAsync();

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

        public async Task<BookingDTO> BookTicket(BookTicketDTO bookTicketDto)
        {
            // Validate the schedule exists
            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.ScheduleId == bookTicketDto.ScheduleId);
            if (schedule == null)
                throw new Exception("Schedule not found.");

            // Validate user exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == bookTicketDto.UserId);
            if (user == null)
                throw new Exception("User not found.");

            // Ensure the bus has seats available
            var availableSeats = await _context.Seats
                .Where(seat => seat.BusId == schedule.BusId && seat.IsAvailable)
                .ToListAsync();

            if (availableSeats == null || !availableSeats.Any())
                throw new Exception("No seats available for the specified bus.");

            // Validate seat availability
            var unavailableSeats = bookTicketDto.SelectedSeats
                .Except(availableSeats.Select(s => s.SeatNumber))
                .ToList();

            if (unavailableSeats.Any())
                throw new Exception($"Some seats are not available: {string.Join(", ", unavailableSeats)}");

            // Create booking
            var booking = new Booking
            {
                UserId = bookTicketDto.UserId,
                ScheduleId = bookTicketDto.ScheduleId,
                BookingDate = DateTime.Now,
                TotalFare = schedule.Fare * bookTicketDto.SelectedSeats.Count,
                Status = "confirmed"
            };

            _context.Bookings.Add(booking);

            // Save changes to generate BookingId
            await _context.SaveChangesAsync();

            // Reserve seats and update BookingId in Seats table
            foreach (var seatNumber in bookTicketDto.SelectedSeats)
            {
                var seat = await _context.Seats.FirstAsync(s => s.SeatNumber == seatNumber && s.BusId == schedule.BusId);
                seat.IsAvailable = false;
                seat.BookingId = booking.BookingId; 
            }

            await _context.SaveChangesAsync();

            return new BookingDTO
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                ScheduleId = booking.ScheduleId,
                ReservedSeats = bookTicketDto.SelectedSeats,
                TotalFare = booking.TotalFare,
                Status = booking.Status,
                BookingDate = booking.BookingDate
            };
        }


        public async Task<bool> CancelBooking(int bookingId)
        {
            var booking = await _context.Bookings.Include(b => b.Seats)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return false;

            booking.Status = "cancelled";

            foreach (var seat in booking.Seats)
            {
                seat.IsAvailable = true;  
                seat.BookingId = null;    
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingDTO>> ViewBookings(int userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Seats)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return bookings.Select(booking => new BookingDTO
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                ScheduleId = booking.ScheduleId,
                ReservedSeats = booking.Seats.Select(s => s.SeatNumber).ToList(), 
                TotalFare = booking.TotalFare,
                Status = booking.Status,
                BookingDate = booking.BookingDate
            });
        }
    }
}

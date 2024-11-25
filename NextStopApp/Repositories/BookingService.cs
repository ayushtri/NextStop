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
            var schedule = await _context.Schedules.Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.ScheduleId == bookTicketDto.ScheduleId);
            if (schedule == null)
                throw new Exception("Schedule not found.");

            // Validate user exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == bookTicketDto.UserId);
            if (user == null)
                throw new Exception("User not found.");

            // Validate seat availability
            var unavailableSeats = await _context.Seats
                .Where(seat => seat.BusId == schedule.BusId && bookTicketDto.SelectedSeats.Contains(seat.SeatNumber) && !seat.IsAvailable)
                .ToListAsync();

            if (unavailableSeats.Any())
                throw new Exception("Some seats are not available.");

            // Reserve seats
            foreach (var seatNumber in bookTicketDto.SelectedSeats)
            {
                var seat = await _context.Seats.FirstAsync(s => s.SeatNumber == seatNumber && s.BusId == schedule.BusId);
                seat.IsAvailable = false;
            }

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

            // Free up seats
            foreach (var seat in booking.Seats)
            {
                seat.IsAvailable = true;
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
                ReservedSeats = booking.Seats.Select(s => s.SeatNumber).ToList(), // Return SeatNumbers instead of SeatIds
                TotalFare = booking.TotalFare,
                Status = booking.Status,
                BookingDate = booking.BookingDate
            });
        }
    }
}

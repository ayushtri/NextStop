using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextStopApp.Repositories
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly NextStopDbContext _context;

        public AdminDashboardService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> ViewAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(user => new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive
            });
        }

        public async Task<bool> AssignRole(AssignRoleDTO assignRoleDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == assignRoleDto.UserId);
            if (user == null)
                throw new Exception("User not found.");

            user.Role = assignRoleDto.Role;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ReportDTO> GenerateReports(GenerateReportsDTO reportDto)
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Route)
                .Include(b => b.Schedule.Bus)
                    .ThenInclude(bus => bus.Operator)
                .Include(b => b.Seats)
                .Where(b => b.Schedule.Date >= reportDto.StartDate &&
                            b.Schedule.Date <= reportDto.EndDate &&
                            (b.Schedule.Route.Origin == reportDto.Route || b.Schedule.Route.Destination == reportDto.Route) && // Check Origin OR Destination
                            b.Schedule.Bus.Operator.Name == reportDto.Operator);

            var bookings = await bookingsQuery.ToListAsync();

            // Calculate total bookings and revenue
            var totalBookings = bookings.Count;
            var totalRevenue = bookings.Sum(b => b.TotalFare);

            // Prepare the report
            var report = new ReportDTO
            {
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                Route = reportDto.Route,
                Operator = reportDto.Operator,
                BookingDetails = bookings.Select(b => new BookingDetailDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    ScheduleId = b.ScheduleId,
                    ReservedSeats = b.Seats.Select(s => s.SeatNumber).ToList(),
                    TotalFare = b.TotalFare,
                    Status = b.Status,
                    BookingDate = b.BookingDate
                }).ToList()
            };

            return report;
        }


    }
}

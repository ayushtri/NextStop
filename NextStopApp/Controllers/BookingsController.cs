using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextStopApp.DTOs;
using NextStopApp.Repositories;

namespace NextStopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private static readonly ILog _log = LogManager.GetLogger(typeof(BookingsController));

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("SearchBus")]
        public async Task<IActionResult> SearchBus([FromBody] SearchBusDTO searchBusDto)
        {
            try
            {
                var schedules = await _bookingService.SearchBus(searchBusDto);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred while searching for buses.", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("BookTicket")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketDTO bookTicketDto)
        {
            try
            {
                var booking = await _bookingService.BookTicket(bookTicketDto);
                return Ok(booking);
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred while booking a ticket.", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CancelBooking/{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                var isCancelled = await _bookingService.CancelBooking(bookingId);
                if (!isCancelled)
                {
                    return NotFound("Booking not found.");
                }

                return Ok("Booking cancelled successfully.");
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while cancelling booking with ID {bookingId}.", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ViewBookings/{userId}")]
        public async Task<IActionResult> ViewBookings(int userId)
        {
            try
            {
                var bookings = await _bookingService.ViewBookings(userId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while viewing bookings for user ID {userId}.", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}

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
                return BadRequest(ex.Message);
            }
        }
    }
}

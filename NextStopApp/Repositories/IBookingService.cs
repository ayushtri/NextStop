using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface IBookingService
    {
        Task<IEnumerable<ScheduleDTO>> SearchBus(SearchBusDTO searchBusDto);
        Task<BookingDTO> BookTicket(BookTicketDTO bookTicketDto);
        Task<bool> CancelBooking(int bookingId);
        Task<IEnumerable<BookingDTO>> ViewBookings(int userId);
    }
}

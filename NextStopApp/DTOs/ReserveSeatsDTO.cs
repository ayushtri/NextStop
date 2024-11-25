namespace NextStopApp.DTOs
{
    public class ReserveSeatsDTO
    {
        public int ScheduleId { get; set; }
        public List<string> SeatNumbers { get; set; }
    }
}

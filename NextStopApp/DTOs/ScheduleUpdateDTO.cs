namespace NextStopApp.DTOs
{
    public class ScheduleUpdateDTO
    {
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public decimal? Fare { get; set; }
        public DateTime? Date { get; set; }
    }
}

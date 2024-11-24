namespace NextStopApp.DTOs
{
    public class RouteDTO
    {
        public int RouteId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public decimal Distance { get; set; }
        public string EstimatedTime { get; set; }
    }

}

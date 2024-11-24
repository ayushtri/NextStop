namespace NextStopApp.DTOs
{
    public class RouteUpdateDTO
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public decimal? Distance { get; set; }
        public string EstimatedTime { get; set; }
    }

}

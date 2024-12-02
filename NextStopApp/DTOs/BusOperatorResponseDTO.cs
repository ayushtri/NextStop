using NextStopApp.Models;

namespace NextStopApp.DTOs
{
    public class BusOperatorResponseDTO
    {
        public string Message { get; set; }
        public BusOperator Operator { get; set; }
    }
}

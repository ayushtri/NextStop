using System.ComponentModel.DataAnnotations;

namespace NextStopApp.DTOs
{
    public class AddSeatsDTO
    {
        [Required]
        public int BusId { get; set; }

        [Required]
        public List<string> SeatNumbers { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace NextStopApp.DTOs
{
    public class BusUpdateDTO
    {
        [StringLength(100)]
        public string BusName { get; set; }

        [StringLength(50)]
        public string BusType { get; set; }

        [Range(1, int.MaxValue)]
        public int? TotalSeats { get; set; }

        public string Amenities { get; set; }
    }
}

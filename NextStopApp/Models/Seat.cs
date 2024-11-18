using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopApp.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }

        [ForeignKey("Bus")]
        public int BusId { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; }

        public bool IsAvailable { get; set; } = true;

        [ForeignKey("Booking")]
        public int? BookingId { get; set; } 

        // Navigation properties
        public Bus Bus { get; set; }
        public Booking Booking { get; set; }
    }
}

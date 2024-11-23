using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopApp.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Schedule")]
        public int ScheduleId { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalFare { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^(confirmed|cancelled)$", ErrorMessage = "Status must be 'confirmed' or 'cancelled'.")]
        public string Status { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Schedule Schedule { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public Payment Payment { get; set; }
    }
}

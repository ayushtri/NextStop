using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopApp.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^(successful|failed)$", ErrorMessage = "PaymentStatus must be 'successful' or 'failed'.")]
        public string PaymentStatus { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
    }
}

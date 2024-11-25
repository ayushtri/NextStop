using System.ComponentModel.DataAnnotations;

namespace NextStopApp.DTOs
{
    public class InitiatePaymentDTO
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression("^(successful|failed)$", ErrorMessage = "PaymentStatus must be 'successful' or 'failed'.")]
        public string PaymentStatus { get; set; }
    }
}

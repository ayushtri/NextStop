using System.ComponentModel.DataAnnotations;

namespace NextStopApp.DTOs
{
    public class BusOperatorRegisterDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(15)]
        public string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Address { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace NextStopApp.Models
{
    public class User
    {

        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(15)]
        [Phone]
        public string Phone { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^(passenger|operator|admin)$", ErrorMessage = "Role must be 'passenger', 'operator', or 'admin'.")]
        public string Role { get; set; }

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<AdminAction> AdminActions { get; set; }
    }
}

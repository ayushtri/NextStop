using System.ComponentModel.DataAnnotations;

namespace NextStopApp.Models
{
    public class BusOperator
    {
        [Key]
        public int OperatorId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(15)]
        [Phone]
        public string ContactNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        // Navigation properties
        public ICollection<Bus> Buses { get; set; }
    }
}

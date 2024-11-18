using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopApp.Models
{
    public class Bus
    {
        [Key]
        public int BusId { get; set; }

        [ForeignKey("Operator")]
        public int OperatorId { get; set; }

        [StringLength(100)]
        public string BusName { get; set; }

        [Required]
        [StringLength(50)]
        public string BusNumber { get; set; }

        [StringLength(50)]
        public string BusType { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TotalSeats must be greater than 0.")]
        public int TotalSeats { get; set; }

        [StringLength(255)]
        public string Amenities { get; set; }

        // Navigation properties
        public BusOperator Operator { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
    }
}

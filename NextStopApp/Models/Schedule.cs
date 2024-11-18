using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopApp.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [ForeignKey("Bus")]
        public int BusId { get; set; }

        [ForeignKey("Route")]
        public int RouteId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Fare { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation properties
        public Bus Bus { get; set; }
        public Route Route { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}

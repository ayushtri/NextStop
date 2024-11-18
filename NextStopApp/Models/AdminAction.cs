using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopApp.Models
{
    public class AdminAction
    {
        [Key]
        public int ActionId { get; set; }

        [ForeignKey("Admin")]
        public int? AdminId { get; set; }

        [StringLength(100)]
        public string ActionType { get; set; }

        public DateTime ActionTimestamp { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string Details { get; set; }

        // Navigation properties
        public User Admin { get; set; }
    }
}

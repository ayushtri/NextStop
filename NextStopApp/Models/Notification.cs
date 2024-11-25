using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextStopApp.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Message { get; set; }

        [Required]
        public DateTime SentDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string NotificationType { get; set; } // e.g., Email, SMS, Push

        // Navigation property
        public User User { get; set; }
    }
}

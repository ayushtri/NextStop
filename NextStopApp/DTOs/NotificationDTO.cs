namespace NextStopApp.DTOs
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public string NotificationType { get; set; }
    }
}

﻿namespace NextStopApp.DTOs
{
    public class SendNotificationDTO
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
    }
}
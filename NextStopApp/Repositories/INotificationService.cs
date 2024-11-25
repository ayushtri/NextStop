using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public interface INotificationService
    {
        Task<bool> SendNotification(SendNotificationDTO sendNotificationDto);
        Task<IEnumerable<NotificationDTO>> ViewNotifications(int userId);
    }
}

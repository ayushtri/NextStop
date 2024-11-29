using NextStopApp.Data;
using NextStopApp.DTOs;
using NextStopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace NextStopApp.Repositories
{
    public class NotificationService : INotificationService
    {
        private readonly NextStopDbContext _context;

        public NotificationService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SendNotification(SendNotificationDTO sendNotificationDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == sendNotificationDto.UserId);
            if (user == null)
                throw new Exception("User not found.");

            
            var notification = new Notification
            {
                UserId = sendNotificationDto.UserId,
                Message = sendNotificationDto.Message,
                NotificationType = sendNotificationDto.NotificationType
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<NotificationDTO>> ViewNotifications(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentDate)
                .ToListAsync();

            return notifications.Select(notification => new NotificationDTO
            {
                NotificationId = notification.NotificationId,
                Message = notification.Message,
                SentDate = notification.SentDate,
                NotificationType = notification.NotificationType
            });
        }
    }
}

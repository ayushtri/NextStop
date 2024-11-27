using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopApp.DTOs;
using NextStopApp.Repositories;

namespace NextStopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private static readonly ILog _log = LogManager.GetLogger(typeof(NotificationsController));

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationDTO sendNotificationDto)
        {
            try
            {
                var isSent = await _notificationService.SendNotification(sendNotificationDto);
                if (!isSent)
                {
                    _log.Error($"Error occurred while sending notification. DateTime: {DateTime.UtcNow}");
                    return BadRequest("Failed to send notification.");
                }

                return Ok("Notification sent successfully.");
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred while sending notification.", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ViewNotifications/{userId}")]
        public async Task<IActionResult> ViewNotifications(int userId)
        {
            try
            {
                var notifications = await _notificationService.ViewNotifications(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _log.Error($"Error occurred while retrieving notifications for user {userId}.", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}

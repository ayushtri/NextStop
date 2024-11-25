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
                    return BadRequest("Failed to send notification.");
                }

                return Ok("Notification sent successfully.");
            }
            catch (Exception ex)
            {
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
                return BadRequest(ex.Message);
            }
        }
    }
}

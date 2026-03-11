using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // POST: api/Notification/send
        [HttpPost("send")]
        public async Task<ActionResult<ApiResponse<bool>>> SendNotification([FromBody] CreateNotificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid notification data",
                    Data = false
                });

            try
            {
                await _notificationService.SendNotificationAsync(dto);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Notification sent successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while sending notification",
                    Data = ex.Message
                });
            }
        }

        // GET: api/Notification/recipients
        [HttpGet("recipients")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<NotificationAgencyUserDto>>>> GetNotificationRecipients()
        {
            var recipients = (await _notificationService.GetNotificationRecipientsAsync()).ToList();
            return Ok(new ApiResponse<IReadOnlyList<NotificationAgencyUserDto>>
            {
                Success = true,
                Message = "Recipients retrieved successfully",
                Data = recipients,
                Meta = new { Count = recipients.Count }
            });
        }

        // GET: api/Notification/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserNotificationDto>>>> GetUserNotifications(Guid userId)
        {
            var notifications = (await _notificationService.GetNotifications(userId)).ToList();
            if (!notifications.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No notifications found for this user",
                    Data = false
                });
            }

            return Ok(new ApiResponse<IReadOnlyList<UserNotificationDto>>
            {
                Success = true,
                Message = "Notifications retrieved successfully",
                Data = notifications,
                Meta = new { Count = notifications.Count }
            });
        }

        // POST: api/Notification/markAsRead/{notificationRecipientId}
        [HttpPost("markAsRead/{notificationRecipientId}")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(Guid notificationRecipientId)
        {
            await _notificationService.MarkAsRead(notificationRecipientId);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Notification marked as read",
                Data = true
            });
        }

        // POST: api/Notification/markAllAsRead/{userId}
        [HttpPost("markAllAsRead/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAllAsRead(Guid userId)
        {
            await _notificationService.MarkAllAsRead(userId);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "All notifications marked as read",
                Data = true
            });
        }
    }
}

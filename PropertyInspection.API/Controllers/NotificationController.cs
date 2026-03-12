using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.Application.IServices;
using PropertyInspection.API.Extensions;
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
            var result = await _notificationService.SendNotificationAsync(dto);
            return this.ToActionResult(result);
        }

        // GET: api/Notification/recipients
        [HttpGet("recipients")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<NotificationAgencyUserDto>>>> GetNotificationRecipients()
        {
            var result = await _notificationService.GetNotificationRecipientsAsync();
            return this.ToActionResult(result, new { Count = result.Data?.Count ?? 0 });
        }

        // GET: api/Notification/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserNotificationDto>>>> GetUserNotifications(Guid userId)
        {
            var result = await _notificationService.GetNotifications(userId);
            return this.ToActionResult(result, new { Count = result.Data?.Count ?? 0 });
        }

        // POST: api/Notification/markAsRead/{notificationRecipientId}
        [HttpPost("markAsRead/{notificationRecipientId}")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(Guid notificationRecipientId)
        {
            var result = await _notificationService.MarkAsRead(notificationRecipientId);
            return this.ToActionResult(result);
        }

        // POST: api/Notification/markAllAsRead/{userId}
        [HttpPost("markAllAsRead/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAllAsRead(Guid userId)
        {
            var result = await _notificationService.MarkAllAsRead(userId);
            return this.ToActionResult(result);
        }
    }
}

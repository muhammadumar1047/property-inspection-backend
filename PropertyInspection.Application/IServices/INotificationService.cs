using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface INotificationService
    {
        /// <summary>
        /// Send a notification to selected users and save recipients
        /// </summary>
        Task<PropertyInspection.Shared.ServiceResponse<bool>> SendNotificationAsync(CreateNotificationDto dto);

        /// <summary>
        /// Get all agencies with their users for notification selection
        /// </summary>
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<NotificationAgencyUserDto>>> GetNotificationRecipientsAsync();

        /// <summary>
        /// Get all notifications for a specific user
        /// </summary>
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<UserNotificationDto>>> GetNotifications(Guid userId);

        /// <summary>
        /// Mark a specific notification as read
        /// </summary>
        Task<PropertyInspection.Shared.ServiceResponse<bool>> MarkAsRead(Guid notificationRecipientId);

        /// <summary>
        /// Mark all notifications as read for a user
        /// </summary>
        Task<PropertyInspection.Shared.ServiceResponse<bool>> MarkAllAsRead(Guid userId);
    }
}

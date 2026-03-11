using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Application.IServices.Notification.Hubs;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace PropertyInspection.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;
      

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task SendNotificationAsync(CreateNotificationDto dto)
        {
            var notification = _mapper.Map<Notification>(dto);
            notification.CreatedAt = DateTime.UtcNow;

            // Step 2: Save notification in DB
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.CommitAsync();

            // Step 3: Get users by SelectedUserIds
            var users = await _unitOfWork.NotificationRecipients.GetAsync(
                u => dto.UserIds.Contains(u.UserId),
                include: q => q // optional include if you need agency/user navigation
            );

            // Step 4: Add recipients
            var recipients = users.Select(u => new NotificationRecipient
            {
                NotificationId = notification.Id,
                AgencyId = u.AgencyId,
                UserId = u.UserId,
                IsRead = false
            }).ToList();

            // Step 5: Send SignalR notifications
            foreach (var user in users)
            {
                await _hubContext.Clients.Group(user.UserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        notification.Id,
                        notification.Title,
                        notification.Message,
                        notification.CreatedAt
                    });
            }

            // Step 6: Save recipients
            //await _unitOfWork.NotificationRecipients.AddAsync(recipients);
            //await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<NotificationAgencyUserDto>> GetNotificationRecipientsAsync()
        {
            // Get all agencies with users (custom repo method)
            return await GetAgenciesWithUsersAsync();
        }

        public async Task<List<UserNotificationDto>> GetNotifications(Guid userId)
        {
            var notifications = await _unitOfWork.NotificationRecipients.GetAsync(
                r => r.UserId == userId,
                include: q => q.Include(n => n.Notification) // Include navigation if needed
            );

            return _mapper.Map<List<UserNotificationDto>>(notifications);
        }

        public async Task MarkAsRead(Guid notificationRecipientId)
        {
            var recipient = await _unitOfWork.NotificationRecipients.GetByIdAsync(notificationRecipientId);
            if (recipient != null)
            {
                recipient.IsRead = true;
                await _unitOfWork.NotificationRecipients.UpdateAsync(recipient);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task MarkAllAsRead(Guid userId)
        {
            var recipients = await _unitOfWork.NotificationRecipients.GetAsync(r => r.UserId == userId && !r.IsRead);

            foreach (var recipient in recipients)
            {
                recipient.IsRead = true;
                await _unitOfWork.NotificationRecipients.UpdateAsync(recipient);
            }

            await _unitOfWork.CommitAsync();
        }

        private async Task<IEnumerable<NotificationAgencyUserDto>> GetAgenciesWithUsersAsync()
        {
            // Get all agencies with their users using GenericRepository
            var agencies = await _unitOfWork.Agencies.GetAsync(
                include: q => q.Include(a => a.Users)
            );

            return _mapper.Map<List<NotificationAgencyUserDto>>(agencies);
        }

    }
}

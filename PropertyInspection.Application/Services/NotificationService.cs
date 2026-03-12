using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Application.IServices.Notification.Hubs;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;
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

        public async Task<ServiceResponse<bool>> SendNotificationAsync(CreateNotificationDto dto)
        {
            try
            {
                if (dto == null || dto.UserIds == null || dto.UserIds.Count == 0)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var notification = _mapper.Map<Notification>(dto);
                notification.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.CommitAsync();

                var users = await _unitOfWork.NotificationRecipients.GetAsync(
                    u => dto.UserIds.Contains(u.UserId),
                    include: q => q
                );

                var recipients = users.Select(u => new NotificationRecipient
                {
                    NotificationId = notification.Id,
                    AgencyId = u.AgencyId,
                    UserId = u.UserId,
                    IsRead = false
                }).ToList();

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

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Notification sent successfully",
                    Data = true
                };
            }
            catch
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<IReadOnlyList<NotificationAgencyUserDto>>> GetNotificationRecipientsAsync()
        {
            try
            {
                var recipients = await GetAgenciesWithUsersAsync();
                return new ServiceResponse<IReadOnlyList<NotificationAgencyUserDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = recipients.ToList()
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<NotificationAgencyUserDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<IReadOnlyList<UserNotificationDto>>> GetNotifications(Guid userId)
        {
            try
            {
                var notifications = await _unitOfWork.NotificationRecipients.GetAsync(
                    r => r.UserId == userId,
                    include: q => q.Include(n => n.Notification)
                );

                return new ServiceResponse<IReadOnlyList<UserNotificationDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<List<UserNotificationDto>>(notifications)
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<UserNotificationDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> MarkAsRead(Guid notificationRecipientId)
        {
            try
            {
                var recipient = await _unitOfWork.NotificationRecipients.GetByIdAsync(notificationRecipientId);
                if (recipient == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                recipient.IsRead = true;
                await _unitOfWork.NotificationRecipients.UpdateAsync(recipient);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record updated successfully",
                    Data = true
                };
            }
            catch
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> MarkAllAsRead(Guid userId)
        {
            try
            {
                var recipients = await _unitOfWork.NotificationRecipients.GetAsync(r => r.UserId == userId && !r.IsRead);

                foreach (var recipient in recipients)
                {
                    recipient.IsRead = true;
                    await _unitOfWork.NotificationRecipients.UpdateAsync(recipient);
                }

                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Records updated successfully",
                    Data = true
                };
            }
            catch
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
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

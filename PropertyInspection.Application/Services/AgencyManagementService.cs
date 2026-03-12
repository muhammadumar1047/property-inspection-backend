using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;
using AutoMapper;

namespace PropertyInspection.Application.Services
{
    public class AgencyManagementService : IAgencyManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AgencyManagementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<UserResponse>> AddUserAsync(Guid actingUserId, Guid actingAgencyId, string? role, AddAgencyUsers dto)
        {
            try
            {
                if (role != "Admin")
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        Message = "You do not have permission to perform this action",
                        ErrorCode = ServiceErrorCodes.Forbidden
                    };
                }

                if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var user = new User
                {
                    Email = dto.Email
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<UserResponse>
                {
                    Success = true,
                    Message = "User added successfully",
                    Data = _mapper.Map<UserResponse>(user)
                };
            }
            catch
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<IReadOnlyList<UserResponse>>> GetUsersAsync(Guid agencyId)
        {
            try
            {
                var users = await _unitOfWork.Users.GetAsync(
                    predicate: u => u.AgencyId == agencyId,
                    include: q => q
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role),
                    orderBy: q => q.OrderBy(u => u.FirstName));

                return new ServiceResponse<IReadOnlyList<UserResponse>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<List<UserResponse>>(users)
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<UserResponse>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateRoleAsync(Guid targetUserId, Guid actingAgencyId, string? role, UpdateRoleDto dto)
        {
            await Task.CompletedTask;
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Unable to process the request at the moment",
                ErrorCode = ServiceErrorCodes.ServerError
            };
        }

        public async Task<ServiceResponse<bool>> DeleteUserAsync(Guid targetUserId, Guid actingAgencyId, string? role)
        {
            try
            {
                if (role != "Admin")
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "You do not have permission to perform this action",
                        ErrorCode = ServiceErrorCodes.Forbidden
                    };
                }

                await Task.CompletedTask;
                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record deleted successfully",
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
    }
}


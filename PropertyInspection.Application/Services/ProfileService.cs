using Microsoft.AspNetCore.Identity;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.Services
{
    public class ProfileService : IMobileProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ServiceResponse<UserProfileDto>> GetProfileAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ServiceResponse<UserProfileDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<UserProfileDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = new UserProfileDto
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        ProfileImage = user.ProfileImage ?? string.Empty
                    }
                };
            }
            catch
            {
                return new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<UserProfileDto>> UpdateProfileAsync(Guid userId, string identityUserId, UpdateProfileRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new ServiceResponse<UserProfileDto>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ServiceResponse<UserProfileDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                if (!string.IsNullOrWhiteSpace(request.Email) &&
                    !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var identityUser = await _userManager.FindByIdAsync(identityUserId);
                    if (identityUser == null)
                    {
                        return new ServiceResponse<UserProfileDto>
                        {
                            Success = false,
                            Message = "Record not found",
                            ErrorCode = ServiceErrorCodes.NotFound
                        };
                    }

                    var setEmailResult = await _userManager.SetEmailAsync(identityUser, request.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        return new ServiceResponse<UserProfileDto>
                        {
                            Success = false,
                            Message = "Unable to process the request at the moment",
                            ErrorCode = ServiceErrorCodes.ServerError
                        };
                    }

                    var setUserNameResult = await _userManager.SetUserNameAsync(identityUser, request.Email);
                    if (!setUserNameResult.Succeeded)
                    {
                        return new ServiceResponse<UserProfileDto>
                        {
                            Success = false,
                            Message = "Unable to process the request at the moment",
                            ErrorCode = ServiceErrorCodes.ServerError
                        };
                    }

                    user.Email = request.Email;
                }

                if (request.FirstName != null)
                {
                    user.FirstName = request.FirstName;
                }

                if (request.LastName != null)
                {
                    user.LastName = request.LastName;
                }

                if (request.ProfileImage != null)
                {
                    user.ProfileImage = request.ProfileImage;
                }

                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = userId;

                await _unitOfWork.CommitAsync();

                return new ServiceResponse<UserProfileDto>
                {
                    Success = true,
                    Message = "Record updated successfully",
                    Data = new UserProfileDto
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        ProfileImage = user.ProfileImage ?? string.Empty
                    }
                };
            }
            catch
            {
                return new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(string identityUserId, ChangePasswordRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var identityUser = await _userManager.FindByIdAsync(identityUserId);
                if (identityUser == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                var result = await _userManager.ChangePasswordAsync(identityUser, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid credentials",
                        ErrorCode = ServiceErrorCodes.Unauthorized
                    };
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Password updated successfully",
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

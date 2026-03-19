using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IMobileProfileService
    {
        Task<ServiceResponse<UserProfileDto>> GetProfileAsync(Guid userId);
        Task<ServiceResponse<UserProfileDto>> UpdateProfileAsync(Guid userId, string identityUserId, UpdateProfileRequest request);
        Task<ServiceResponse<bool>> ChangePasswordAsync(string identityUserId, ChangePasswordRequest request);
    }
}

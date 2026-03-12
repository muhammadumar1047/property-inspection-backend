using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;


namespace PropertyInspection.Application.IServices
{
    public interface IAgencyManagementService
    {
        Task<ServiceResponse<UserResponse>> AddUserAsync(Guid actingUserId, Guid actingAgencyId, string? role, AddAgencyUsers dto);
        Task<ServiceResponse<IReadOnlyList<UserResponse>>> GetUsersAsync(Guid agencyId);
        Task<ServiceResponse<bool>> UpdateRoleAsync(Guid targetUserId, Guid actingAgencyId, string? role, UpdateRoleDto dto);
        Task<ServiceResponse<bool>> DeleteUserAsync(Guid targetUserId, Guid actingAgencyId, string? role);
    }
}


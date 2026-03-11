using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;


namespace PropertyInspection.Application.IServices
{
    public interface IAgencyManagementService
    {
        Task<UserDto> AddUserAsync(Guid actingUserId, Guid actingAgencyId, string? role, AddAgencyUsers dto);
        Task<List<UserDto>> GetUsersAsync(Guid agencyId);
        Task<bool> UpdateRoleAsync(Guid targetUserId, Guid actingAgencyId, string? role, UpdateRoleDto dto);
        Task<bool> DeleteUserAsync(Guid targetUserId, Guid actingAgencyId, string? role);
    }
}

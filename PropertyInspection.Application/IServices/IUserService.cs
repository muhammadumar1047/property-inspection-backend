using PropertyInspection.Shared;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetUsersAsync(
            UserFilterDto filter,
            Guid? agencyId,
            int page = 1,
            int pageSize = 10);
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task UpdateAsync(Guid id, UpdateUserDto dto, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
        Task<UserDto?> GetUserWithRolesByIdentityIdAsync(string identityUserId);
    }
}

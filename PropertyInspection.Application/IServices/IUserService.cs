using PropertyInspection.Shared;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IUserService
    {
        Task<ServiceResponse<PagedResult<UserResponse>>> GetUsersAsync(
            UserFilterDto filter,
            Guid? agencyId,
            int page = 1,
            int pageSize = 10);
        Task<ServiceResponse<UserResponse>> GetByIdAsync(Guid id);
        Task<ServiceResponse<UserResponse>> CreateAsync(CreateUserRequest dto);
        Task<ServiceResponse<bool>> UpdateAsync(Guid id, UpdateUserRequest dto, Guid userId);
        Task<ServiceResponse<bool>> DeleteAsync(Guid id, Guid userId);
        Task<ServiceResponse<UserResponse>> GetUserWithRolesByIdentityIdAsync(string identityUserId);
    }
}


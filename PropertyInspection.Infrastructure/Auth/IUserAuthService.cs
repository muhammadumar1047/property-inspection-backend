using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Infrastructure.Auth
{
    public interface IUserAuthService
    {
        Task<PropertyInspection.Shared.ServiceResponse<ApplicationUser>> LoginAsync(string email, string password);
        Task<PropertyInspection.Shared.ServiceResponse<bool>> LogoutAsync();
        Task<PropertyInspection.Shared.ServiceResponse<bool>> VerifyMfaAsync(string userId, string token);
        Task<PropertyInspection.Shared.ServiceResponse<ApplicationUser>> CreateUserAsync(ApplicationUser identityUser, string password);
        Task<PropertyInspection.Shared.ServiceResponse<bool>> UpdateUserPasswordAsync(string email, string newPassword);
        Task<PropertyInspection.Shared.ServiceResponse<bool>> DeleteUserAsync(ApplicationUser user);

    }
}

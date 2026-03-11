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
        Task<ApplicationUser> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<bool> VerifyMfaAsync(string userId, string token);
        Task<ApplicationUser> CreateUserAsync(ApplicationUser identityUser, string password);
        Task<IdentityResult> UpdateUserPasswordAsync(string email, string newPassword);
        Task<bool> DeleteUserAsync(ApplicationUser user);

    }
}

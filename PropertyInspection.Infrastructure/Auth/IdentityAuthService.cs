using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Infrastructure.Auth
{
    public class IdentityAuthService : IUserAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityAuthService(SignInManager<ApplicationUser> signInManager,
                                   UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
            if (result.Succeeded) return user;
            return null;
        }

        public async Task LogoutAsync() => await _signInManager.SignOutAsync();

        public async Task<bool> VerifyMfaAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, token);
        }


        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser identityUser, string password)
        {
            var result = await _userManager.CreateAsync(identityUser, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(e => e.Description));

                throw new Exception($"User creation failed: {errors}");
            }

            return identityUser;
        }

        public async Task<IdentityResult> UpdateUserPasswordAsync(string email, string newPassword)
        {


            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found");


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result;
        }

        public async Task<bool> DeleteUserAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User deletion failed: {errors}");
            }

            return true;
        }
    }
}

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

        public async Task<PropertyInspection.Shared.ServiceResponse<ApplicationUser>> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new PropertyInspection.Shared.ServiceResponse<ApplicationUser>
                    {
                        Success = false,
                        Message = "Invalid credentials",
                        ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.Unauthorized
                    };
                }

                var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
                if (!result.Succeeded)
                {
                    return new PropertyInspection.Shared.ServiceResponse<ApplicationUser>
                    {
                        Success = false,
                        Message = "Invalid credentials",
                        ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.Unauthorized
                    };
                }

                return new PropertyInspection.Shared.ServiceResponse<ApplicationUser>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = user
                };
            }
            catch
            {
                return new PropertyInspection.Shared.ServiceResponse<ApplicationUser>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<PropertyInspection.Shared.ServiceResponse<bool>> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return new PropertyInspection.Shared.ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Logout successful",
                    Data = true
                };
            }
            catch
            {
                return new PropertyInspection.Shared.ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<PropertyInspection.Shared.ServiceResponse<bool>> VerifyMfaAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new PropertyInspection.Shared.ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.NotFound
                    };
                }

                var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, token);
                return new PropertyInspection.Shared.ServiceResponse<bool>
                {
                    Success = isValid,
                    Message = isValid ? "Verification successful" : "Invalid request data",
                    ErrorCode = isValid ? null : PropertyInspection.Shared.ServiceErrorCodes.InvalidRequest,
                    Data = isValid
                };
            }
            catch
            {
                return new PropertyInspection.Shared.ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                };
            }
        }


        public async Task<PropertyInspection.Shared.ServiceResponse<ApplicationUser>> CreateUserAsync(ApplicationUser identityUser, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(identityUser, password);

                if (!result.Succeeded)
                {
                    return new PropertyInspection.Shared.ServiceResponse<ApplicationUser>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                    };
                }

                return new PropertyInspection.Shared.ServiceResponse<ApplicationUser>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = identityUser
                };
            }
            catch
            {
                return new PropertyInspection.Shared.ServiceResponse<ApplicationUser>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<PropertyInspection.Shared.ServiceResponse<bool>> UpdateUserPasswordAsync(string email, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new PropertyInspection.Shared.ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.NotFound
                    };
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (!result.Succeeded)
                {
                    return new PropertyInspection.Shared.ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                    };
                }

                return new PropertyInspection.Shared.ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Password updated successfully",
                    Data = true
                };
            }
            catch
            {
                return new PropertyInspection.Shared.ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<PropertyInspection.Shared.ServiceResponse<bool>> DeleteUserAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                {
                    return new PropertyInspection.Shared.ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.InvalidRequest
                    };
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return new PropertyInspection.Shared.ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                    };
                }

                return new PropertyInspection.Shared.ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record deleted successfully",
                    Data = true
                };
            }
            catch
            {
                return new PropertyInspection.Shared.ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = PropertyInspection.Shared.ServiceErrorCodes.ServerError
                };
            }
        }
    }
}

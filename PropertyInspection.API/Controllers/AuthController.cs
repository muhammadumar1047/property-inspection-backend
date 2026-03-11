using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.Application.IServices;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Shared;
using PropertyInspection.Shared.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IUserAuthService _userAuthService;
        private readonly IPermissionCacheService _permissionCacheService;

        public AuthController(
            IUserService userService,
            IJwtService jwtService,
            IUserAuthService userAuthService,
            IPermissionCacheService permissionCacheService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _userAuthService = userAuthService;
            _permissionCacheService = permissionCacheService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<object>>> Login(LoginDto dto)
        {
            var identityUser = await _userAuthService.LoginAsync(dto.Email, dto.Password);

            if (identityUser == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Data = false
                });
            }

            if (identityUser.TwoFactorEnabled)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "MFA is required",
                    Data = new { mfaRequired = true }
                });
            }

            var domainUser = await _userService.GetUserWithRolesByIdentityIdAsync(identityUser.Id);

            if (domainUser == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Domain user not found",
                    Data = false
                });
            }

            var fullName = string.Join(" ",
                new[] { domainUser.FirstName, domainUser.LastName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Id),
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                new Claim("UserName", identityUser.UserName ?? ""),
                new Claim("Email", identityUser.Email ?? ""),
                new Claim("FullName", fullName),
                new Claim("IsSuperAdmin", domainUser.IsSuperAdmin.ToString()),
                new Claim("IsAgencyAdmin", domainUser.IsAgencyAdmin.ToString()),
                new Claim("DomainUserId", domainUser.Id.ToString()),
            };
            var permissions = new HashSet<string>();

            if (!domainUser.IsSuperAdmin)
            {
                if (domainUser.AgencyId == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Agency missing",
                        Data = false
                    });
                }

                claims.Add(new Claim("AgencyId", domainUser.AgencyId.Value.ToString()));
                claims.Add(new Claim("AgencyName", domainUser.AgencyName ?? ""));

                var userRoles = domainUser.UserRoles ?? Enumerable.Empty<UserRoleDto>();

                foreach (var role in userRoles)
                {
                    if (!string.IsNullOrWhiteSpace(role.RoleName))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.RoleName));

                        //if (role.Permissions != null)
                        //{
                        //    foreach (var permission in role.Permissions)
                        //    {
                        //        if (!string.IsNullOrWhiteSpace(permission.Name))
                        //        {
                        //            permissions.Add(permission.Name);
                        //        }
                        //    }
                        //}
                    }
                }
            }

            await _permissionCacheService.SetPermissionsAsync(domainUser.Id, permissions);

            var token = _jwtService.GenerateJwtToken(claims);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Login successful",
                Data = new
                {
                    Token = token,
                    User = new
                    {
                        IdentityUserId = identityUser.Id,
                        DomainUserId = domainUser.Id,
                        Email = domainUser.Email,
                        FullName = fullName,
                        IsSuperAdmin = domainUser.IsSuperAdmin,
                        Roles = (domainUser.UserRoles ?? Enumerable.Empty<UserRoleDto>()).Select(r => r.RoleName),
                        AgencyId = domainUser.AgencyId,
                        AgencyName = domainUser.AgencyName,
                        FirstName = domainUser.FirstName,
                        LastName = domainUser.LastName,
                        ProfileImage = domainUser.ProfileImage,
                        isAgencyAdmin = domainUser.IsAgencyAdmin
                    }
                },
                Meta = new
                {
                  
                }
            });
        }
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            await _userAuthService.LogoutAsync();
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Logged out successfully",
                Data = true
            });
        }

        [HttpPost("update-password")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePassword([FromBody] UpdatePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = ModelState
                });
            }

            var result = await _userAuthService.UpdateUserPasswordAsync(model.Email, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Password update failed",
                    Data = result.Errors.Select(e => e.Description).ToList()
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Password updated successfully",
                Data = true
            });
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Shared;
using PropertyInspection.Shared.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PropertyInspection.API.Controllaers
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
            var loginResult = await _userAuthService.LoginAsync(dto.Email, dto.Password);
            if (!loginResult.Success || loginResult.Data == null)
            {
                return this.ToActionResult(new ServiceResponse<object>
                {
                    Success = false,
                    Message = loginResult.Message,
                    ErrorCode = loginResult.ErrorCode
                });
            }

            var identityUser = loginResult.Data;

            if (identityUser.TwoFactorEnabled)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "MFA is required",
                    Data = new { mfaRequired = true }
                });
            }

            var domainUserResult = await _userService.GetUserWithRolesByIdentityIdAsync(identityUser.Id);
            if (!domainUserResult.Success || domainUserResult.Data == null)
            {
                return this.ToActionResult(new ServiceResponse<object>
                {
                    Success = false,
                    Message = domainUserResult.Message,
                    ErrorCode = domainUserResult.ErrorCode == ServiceErrorCodes.NotFound
                        ? ServiceErrorCodes.Unauthorized
                        : domainUserResult.ErrorCode
                });
            }

            var domainUser = domainUserResult.Data;

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
                    return this.ToActionResult(new ServiceResponse<object>
                    {
                        Success = false,
                        Message = "Unauthorized",
                        ErrorCode = ServiceErrorCodes.Unauthorized
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

                        if (role.Permissions != null)
                        {
                            foreach (var permission in role.Permissions)
                            {
                                if (!string.IsNullOrWhiteSpace(permission.Name))
                                {
                                    permissions.Add(permission.Name);
                                }
                            }
                        }
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
            var result = await _userAuthService.LogoutAsync();
            return this.ToActionResult(result);
        }

        [HttpPost("update-password")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePassword([FromBody] UpdatePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return this.ToActionResult(new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var result = await _userAuthService.UpdateUserPasswordAsync(model.Email, model.NewPassword);
            return this.ToActionResult(result);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMobileProfileService _profileService;

        public ProfileController(IMobileProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
        {
            if (!Guid.TryParse(User.GetDomainUserId(), out var userId))
            {
                return this.ToActionResult(new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Unauthorized",
                    ErrorCode = ServiceErrorCodes.Unauthorized
                });
            }

            var result = await _profileService.GetProfileAsync(userId);
            return this.ToActionResult(result);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.ToActionResult(new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            if (!Guid.TryParse(User.GetDomainUserId(), out var userId))
            {
                return this.ToActionResult(new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Unauthorized",
                    ErrorCode = ServiceErrorCodes.Unauthorized
                });
            }

            var identityUserId = User.GetIdentityUserId();
            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return this.ToActionResult(new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Unauthorized",
                    ErrorCode = ServiceErrorCodes.Unauthorized
                });
            }

            var result = await _profileService.UpdateProfileAsync(userId, identityUserId, request);
            return this.ToActionResult(result);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequest request)
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

            var identityUserId = User.GetIdentityUserId();
            if (string.IsNullOrWhiteSpace(identityUserId))
            {
                return this.ToActionResult(new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized",
                    ErrorCode = ServiceErrorCodes.Unauthorized
                });
            }

            var result = await _profileService.ChangePasswordAsync(identityUserId, request);
            return this.ToActionResult(result);
        }
    }
}



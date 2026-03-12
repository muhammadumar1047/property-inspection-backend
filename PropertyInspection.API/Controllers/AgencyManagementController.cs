using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Authorization;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgencyManagementController : ControllerBase
    {
        private readonly IAgencyManagementService _mgmtService;

        public AgencyManagementController(IAgencyManagementService mgmtService)
        {
            _mgmtService = mgmtService;
        }

        [HttpPost("AddUser")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> AddUserToAgency([FromBody] AddAgencyUsers dto)
        {
            if (!ModelState.IsValid)
            {
                return this.ToActionResult(new ServiceResponse<UserResponse>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var role = HttpContext.Items["role"] as string;
            var userId = (Guid)HttpContext.Items["userId"]!;
            var userAgencyId = (Guid)HttpContext.Items["AgencyId"]!;

            var result = await _mgmtService.AddUserAsync(userId, userAgencyId, role, dto);
            return this.ToActionResult(result, new { UserId = result.Data?.Id });
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserResponse>>>> GetUsersInAgency()
        {

            //Guid agencyId = (Guid)HttpContext.Items["AgencyId"]!;
            Guid agencyId = Guid.Parse("a0b4dac3-71f0-4dd3-b9f3-0f09563cff7f");
            var result = await _mgmtService.GetUsersAsync(agencyId);
            return this.ToActionResult(result, new { Count = result.Data?.Count ?? 0 });
        }

        [HttpPut("UpdateRole/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateRole(Guid userId, [FromBody] UpdateRoleDto dto)
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

            var role = HttpContext.Items["role"] as string;
            var userAgencyId = (Guid)HttpContext.Items["AgencyId"]!;

            var result = await _mgmtService.UpdateRoleAsync(userId, userAgencyId, role, dto);
            return this.ToActionResult(result);
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(Guid userId)
        {
            var role = HttpContext.Items["role"] as string;
            var userAgencyId = (Guid)HttpContext.Items["AgencyId"]!;

            var result = await _mgmtService.DeleteUserAsync(userId, userAgencyId, role);
            return this.ToActionResult(result);
        }
    }
}


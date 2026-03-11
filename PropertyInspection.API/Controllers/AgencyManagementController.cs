using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Authorization;
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
        public async Task<ActionResult<ApiResponse<bool>>> AddUserToAgency([FromBody] AddAgencyUsers dto)
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

            var role = HttpContext.Items["role"] as string;
            var userId = (Guid)HttpContext.Items["userId"]!;
            var userAgencyId = (Guid)HttpContext.Items["AgencyId"]!;

            try
            {
                var user = await _mgmtService.AddUserAsync(userId, userAgencyId, role, dto);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Record created successfully",
                    Data = true,
                    Meta = new { UserId = user.Id }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDto>>>> GetUsersInAgency()
        {

            //Guid agencyId = (Guid)HttpContext.Items["AgencyId"]!;
            Guid agencyId = Guid.Parse("a0b4dac3-71f0-4dd3-b9f3-0f09563cff7f");
            var users = await _mgmtService.GetUsersAsync(agencyId);
            var list = users.ToList();

            return Ok(new ApiResponse<IReadOnlyList<UserDto>>
            {
                Success = true,
                Message = "Records retrieved successfully",
                Data = list,
                Meta = new { Count = list.Count }
            });
        }

        [HttpPut("UpdateRole/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateRole(Guid userId, [FromBody] UpdateRoleDto dto)
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

            var role = HttpContext.Items["role"] as string;
            var userAgencyId = (Guid)HttpContext.Items["AgencyId"]!;

            try
            {
                var updated = await _mgmtService.UpdateRoleAsync(userId, userAgencyId, role, dto);
                if (!updated)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found in your agency.",
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Record updated successfully",
                    Data = true
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(Guid userId)
        {
            var role = HttpContext.Items["role"] as string;
            var userAgencyId = (Guid)HttpContext.Items["AgencyId"]!;

            try
            {
                var deleted = await _mgmtService.DeleteUserAsync(userId, userAgencyId, role);
                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found in your agency.",
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Record deleted successfully",
                    Data = true
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }
    }
}

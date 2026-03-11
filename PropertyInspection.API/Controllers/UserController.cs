using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PropertyInspection.API.Authorization;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.Auth;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetUsers(
           [FromQuery] UserFilterDto filter,
           [FromQuery] Guid? agencyId,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetUsersAsync(
                filter, agencyId, page, pageSize);

            return Ok(new ApiResponse<PagedResult<UserDto>>
            {
                Success = true,
                Message = "Users fetched successfully",
                Data = users
            });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User fetched successfully",
                Data = user
            });
        }

        [HttpPost]
        //[Permission("inspection.create")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto dto)
        {
            var userId = Guid.Parse(User.GetDomainUserId());
            var user = await _userService.CreateAsync(dto);

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User created successfully",
                Data = user
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            Guid userId = Guid.Parse(User.GetDomainUserId());
            await _userService.UpdateAsync(id, dto, userId);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "User updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
        {
            var userId = Guid.Parse(User.GetDomainUserId());
            await _userService.DeleteAsync(id, userId);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "User deleted successfully"
            });
        }
    }
}

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
        public async Task<ActionResult<ApiResponse<PagedResult<UserResponse>>>> GetUsers(
           [FromQuery] UserFilterDto filter,
           [FromQuery] Guid? agencyId,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetUsersAsync(
                filter, agencyId, page, pageSize);
            return this.ToActionResult(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return this.ToActionResult(result);
        }

        [HttpPost]
        //[Permission("inspection.create")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Create([FromBody] CreateUserRequest dto)
        {
            var userId = Guid.Parse(User.GetDomainUserId());
            var result = await _userService.CreateAsync(dto);
            return this.ToActionResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateUserRequest dto)
        {
            Guid userId = Guid.Parse(User.GetDomainUserId());
            var result = await _userService.UpdateAsync(id, dto, userId);
            return this.ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var userId = Guid.Parse(User.GetDomainUserId());
            var result = await _userService.DeleteAsync(id, userId);
            return this.ToActionResult(result);
        }
    }
}


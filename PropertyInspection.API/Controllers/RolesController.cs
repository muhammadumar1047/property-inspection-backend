using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Infrastructure.Data;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/Role?agencyId={agencyId}
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetRoles([FromQuery] Guid? agencyId)
        {
            var response = await _roleService.GetRolesAsync(agencyId);
            return this.ToActionResult(response);
        }

        // GET: api/Role/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetById(Guid id)
        {
            var response = await _roleService.GetByIdAsync(id);
            return this.ToActionResult(response);
        }

        // POST: api/Role
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleDto>>> Create([FromBody] CreateRoleRequest dto)
        {
            var userId = Guid.Parse(User.GetDomainUserId()); // assuming JWT stores UserId in Name
            var response = await _roleService.CreateAsync(dto, userId);
            return this.ToActionResult(response);
        }

        // PUT: api/Role/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateRoleRequest dto)
        {
            var userId = Guid.Parse(User.GetDomainUserId()); // assuming JWT stores UserId in Name
            var response = await _roleService.UpdateAsync(id, dto, userId);
            return this.ToActionResult(response);
        }

        // DELETE: api/Role/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var userId = Guid.Parse(User.Identity.Name); // assuming JWT stores UserId in Name
            var response = await _roleService.DeleteAsync(id, userId);
            return this.ToActionResult(response);
        }


    }
}


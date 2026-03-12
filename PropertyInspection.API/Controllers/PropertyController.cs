using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Authorization;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        //[Permission("property.view")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PropertyResponse>>>> GetAllByAgency(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] PropertyType? propertyType = null,
            [FromQuery] Guid? propertyManagerId = null,
            [FromQuery] string? tenant = null,
            [FromQuery] string? owner = null,
            [FromQuery] string? suburb = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] Guid? agencyId = null)
        {
          

            var result = await _propertyService.GetAllByAgencyAsync(
                agencyId,
                pageNumber,
                pageSize,
                propertyType,
                propertyManagerId,
                tenant,
                owner,
                suburb,
                isActive
            );
            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }

        [Permission("property.view")]
        [HttpGet("{propertyId}")]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> GetById(Guid propertyId, Guid? agencyId)
        {
            var result = await _propertyService.GetByIdAsync(propertyId, agencyId);
            return this.ToActionResult(result);
        }

        [Permission("property.create")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> Create([FromBody] CreatePropertyRequest propertyRequest)
        {
            var result = await _propertyService.CreateAsync(propertyRequest);
            return this.ToCreatedAtActionResult(
                nameof(GetById),
                new { propertyId = result.Data?.Id ?? Guid.Empty },
                result);
        }

        [Permission("property.update")]
        [HttpPut("{propertyId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid propertyId, [FromBody] UpdatePropertyRequest propertyRequest)
        {
            var result = await _propertyService.UpdateAsync(propertyId, propertyRequest);
            return this.ToActionResult(result);
        }

        [Permission("property.delete")]
        [HttpDelete("{propertyId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid propertyId, Guid? agencyId)
        {
            var result = await _propertyService.DeletePropertyAsync(propertyId, agencyId);
            return this.ToActionResult(result);
        }
    }
}


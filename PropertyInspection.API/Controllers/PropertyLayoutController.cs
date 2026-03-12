using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Authorization;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyLayoutController : ControllerBase
    {
        private readonly IPropertyLayoutService _propertyLayoutService;

        public PropertyLayoutController(IPropertyLayoutService propertyLayoutService)
        {
            _propertyLayoutService = propertyLayoutService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PropertyLayoutResponse>>>> GetAllByAgency(
            Guid? agencyId,
            int pageNumber = 1,
            int pageSize = 10
        )
        {
            var result = await _propertyLayoutService.GetAllByAgencyAsync(agencyId, pageNumber, pageSize);
            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }

        [HttpGet("{layoutId}")]
        public async Task<ActionResult<ApiResponse<PropertyLayoutResponse>>> GetById(Guid layoutId , Guid? agencyId)
        {

            var result = await _propertyLayoutService.GetByIdAsync(layoutId, agencyId);
            return this.ToActionResult(result);
        }

        [HttpPost]
        //[Permission("propertylayout.create")]
        public async Task<ActionResult<ApiResponse<PropertyLayoutResponse>>> Create([FromBody] CreatePropertyLayoutRequest layoutDto)
        {
           
            var result = await _propertyLayoutService.CreateAsync(layoutDto);
            return this.ToCreatedAtActionResult(
                nameof(GetById),
                new { layoutId = result.Data?.Id ?? Guid.Empty },
                result);
        }

        [HttpPut("{layoutId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid layoutId, [FromBody] UpdatePropertyLayoutRequest layoutRequest)
        {
            var result = await _propertyLayoutService.UpdateAsync(layoutId, layoutRequest);
            return this.ToActionResult(result);
        }

        [HttpDelete("{layoutId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid layoutId , Guid? agencyId)
        {

            var result = await _propertyLayoutService.DeleteAsync(layoutId, agencyId);
            return this.ToActionResult(result);
        }
    }
}


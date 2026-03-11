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
        public async Task<ActionResult<ApiResponse<PagedResult<PropertyLayoutDto>>>> GetAllByAgency(
            Guid? agencyId,
            int pageNumber = 1,
            int pageSize = 10
        )
        {

            var (layouts, totalCount) = (await _propertyLayoutService.GetAllByAgencyAsync(agencyId));

            var result = new PagedResult<PropertyLayoutDto>
            {
                Data = layouts.ToList(),
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(new ApiResponse<PagedResult<PropertyLayoutDto>>
            {
                Success = true,
                Message = "Records retrieved successfully",
                Data = result,
                Meta = new { result.Data.Count }
            });
        }

        [HttpGet("{layoutId}")]
        public async Task<ActionResult<ApiResponse<PropertyLayoutDto>>> GetById(Guid layoutId , Guid? agencyId)
        {

            var layout = await _propertyLayoutService.GetByIdAsync(layoutId, agencyId);
            if (layout == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Record not found.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<PropertyLayoutDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = layout
            });
        }

        [HttpPost]
        //[Permission("propertylayout.create")]
        public async Task<ActionResult<ApiResponse<PropertyLayoutDto>>> Create([FromBody] CreatePropertyLayoutDto layoutDto)
        {
           
            var created = await _propertyLayoutService.CreateAsync(layoutDto);
            return CreatedAtAction(
                nameof(GetById),
                new { layoutId = created.Id },
                new ApiResponse<PropertyLayoutDto>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = created
                });
        }

        [HttpPut("{layoutId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid layoutId, [FromBody] PropertyLayoutDto layoutDto)
        {
           

            if (layoutId != layoutDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Layout ID mismatch",
                    Data = false
                });
            }

            try
            {
                await _propertyLayoutService.UpdateAsync(layoutDto);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Record updated successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }

        [HttpDelete("{layoutId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid layoutId , Guid? agencyId)
        {

            var deleted = await _propertyLayoutService.DeleteAsync(layoutId, agencyId);
            if (!deleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Record not found.",
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
    }
}

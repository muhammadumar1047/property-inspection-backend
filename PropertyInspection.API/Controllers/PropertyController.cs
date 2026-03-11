using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Authorization;
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
        public async Task<ActionResult<ApiResponse<PagedResult<PropertyDto>>>> GetAllByAgency(
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
          

            var (properties, totalCount) = await _propertyService.GetAllByAgencyAsync(
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

            var result = new PagedResult<PropertyDto>
            {
                Data = properties.ToList(),
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(new ApiResponse<PagedResult<PropertyDto>>
            {
                Success = true,
                Message = "Records retrieved successfully",
                Data = result,
                Meta = new { Count = result.Data.Count }
            });
        }

        [Permission("property.view")]
        [HttpGet("{propertyId}")]
        public async Task<ActionResult<ApiResponse<PropertyDto>>> GetById(Guid propertyId, Guid? agencyId)
        {
            var property = await _propertyService.GetByIdAsync(propertyId,agencyId);
            if (property == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Record not found.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<PropertyDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = property
            });
        }

        [Permission("property.create")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PropertyDto>>> Create([FromBody] PropertyDto propertyDto)
        {
            try
            {
               
                var createdProperty = await _propertyService.CreateAsync(propertyDto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { propertyId = createdProperty.Id },
                    new ApiResponse<PropertyDto>
                    {
                        Success = true,
                        Message = "Entity created successfully",
                        Data = createdProperty
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }

        [Permission("property.update")]
        [HttpPut("{propertyId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid propertyId, [FromBody] PropertyDto propertyDto)
        {
            if (propertyId != propertyDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Property ID mismatch",
                    Data = false
                });
            }

            try
            {
                var updatedProperty = await _propertyService.UpdateAsync(propertyDto);
                if (updatedProperty == null)
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
                    Message = "Record updated successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }

        [Permission("property.delete")]
        [HttpDelete("{propertyId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid propertyId, Guid? agencyId)
        {
            var deleted = await _propertyService.DeletePropertyAsync(propertyId, agencyId);
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

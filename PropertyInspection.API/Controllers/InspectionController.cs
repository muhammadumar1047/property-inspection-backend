using Microsoft.AspNetCore.Http;
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
    public class InspectionController : ControllerBase
    {
        private readonly IInspectionService _inspectionService;

        public InspectionController(IInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<InspectionDto>>>> GetAllInspections(
            Guid? agencyId,
            Guid inspectionId = default,
            int pageNumber = 1,
            int pageSize = 10,
            InspectionType? inspectionType = null,
            InspectionStatus? inspectionStatus = null,
            Guid? inspectorId = null,
            string? suburb = null,
            DateTime? inspectionDate = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? searchProperty = null
        )
        {
            var (inspections, totalCount) = await _inspectionService.GetAllInspectionsAsync(
                agencyId,
                inspectionId, pageNumber, pageSize,
                inspectionType, inspectionStatus, inspectorId,
                suburb, inspectionDate, startDate, endDate, searchProperty
            );

            var result = new PagedResult<InspectionDto>
            {
                Data = inspections.ToList(),
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(new ApiResponse<PagedResult<InspectionDto>>
            {
                Success = true,
                Message = "Records retrieved successfully",
                Data = result,
                Meta = new { Count = result.Data.Count }
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<InspectionDto>>> GetById(Guid id, Guid? agencyId)
        {
            var inspection = await _inspectionService.GetByIdAsync(id , agencyId);
            if (inspection == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Record not found.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<InspectionDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = inspection
            });
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<InspectionDto>>>> GetByProperty(Guid propertyId , Guid? agencyId)
        {
            var inspections = await _inspectionService.GetAllByPropertyAsync(propertyId ,agencyId);
            return Ok(new ApiResponse<IReadOnlyList<InspectionDto>>
            {
                Success = true,
                Message = "Records retrieved successfully",
                Data = inspections,
                Meta = new { Count = inspections.Count }
            });
        }

        [HttpPost]
        [Permission("inspection.create")]
        public async Task<ActionResult<ApiResponse<InspectionDto>>> Create([FromBody] CreateInspectionDto inspectionDto)
        {
            if (inspectionDto == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Inspection data is required.",
                    Data = false
                });
            }

            var created = await _inspectionService.CreateAsync(inspectionDto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new ApiResponse<InspectionDto>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = created
                });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] InspectionDto inspectionDto)
        {
            if (inspectionDto == null || id != inspectionDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid inspection data.",
                    Data = false
                });
            }

            var updated = await _inspectionService.UpdateAsync(inspectionDto);
            if (updated == null)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id , Guid? agencyId)
        {
            var deleted = await _inspectionService.DeleteAsync(id , agencyId);
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

        [HttpDelete("landlord-snapshot/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteLandlordSnapshot(Guid id, Guid? agencyId)
        {
            var deleted = await _inspectionService.DeleteLandlordSnapshotAsync(id, agencyId);
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

        [HttpDelete("tenancy-snapshot/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTenancySnapshot(Guid id, Guid? agencyId)
        {
            var deleted = await _inspectionService.DeleteTenancySnapshotAsync(id, agencyId);
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

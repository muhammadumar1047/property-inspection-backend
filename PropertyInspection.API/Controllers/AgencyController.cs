using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgencyController : ControllerBase
    {
        private readonly IAgencyService _agencyService;

        public AgencyController(IAgencyService agencyService)
        {
            _agencyService = agencyService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AgencyDto>>>> GetAgencies(
            [FromQuery] Guid? countryId,
            [FromQuery] Guid? stateId,
            [FromQuery] string? name,
            [FromQuery] string? suburb,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var (agencies, totalCount) = await _agencyService.GetAllAsync(pageNumber, pageSize, countryId, stateId, name, suburb);

            var result = new PagedResult<AgencyDto>
            {
                Data = agencies,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(new ApiResponse<PagedResult<AgencyDto>>
            {
                Success = true,
                Message = "Records retrieved successfully",
                Data = result,
                Meta = new { Count = result.Data.Count }
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AgencyDto>>> GetAgency(Guid id)
        {
            var agency = await _agencyService.GetByIdAsync(id);
            if (agency == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Agency not found.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<AgencyDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = agency
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AgencyDto>>> CreateAgency([FromBody] CreateAgencyDto dto)
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

            var result = await _agencyService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetAgency),
                new { id = result.Id },
                new ApiResponse<AgencyDto>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = result
                });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AgencyDto>>> UpdateAgency(Guid id, [FromBody] UpdateAgencyDto dto)
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

            try
            {
                var updatedAgency = await _agencyService.UpdateAsync(id, dto);
                if (updatedAgency == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Agency not found.",
                        Data = false
                    });
                }

                return Ok(new ApiResponse<AgencyDto>
                {
                    Success = true,
                    Message = "Record updated successfully",
                    Data = updatedAgency
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = false
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAgency(Guid id)
        {
            
            try
            {
                var success = await _agencyService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Agency not found.",
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Record deleted successfully",
                    Data = true,
                    Meta = new { AgencyId = id }
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

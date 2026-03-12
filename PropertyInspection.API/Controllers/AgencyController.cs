using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
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
        public async Task<ActionResult<ApiResponse<PagedResult<AgencyResponse>>>> GetAgencies(
            [FromQuery] Guid? countryId,
            [FromQuery] Guid? stateId,
            [FromQuery] string? name,
            [FromQuery] string? suburb,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _agencyService.GetAllAsync(pageNumber, pageSize, countryId, stateId, name, suburb);
            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AgencyResponse>>> GetAgency(Guid id)
        {
            var result = await _agencyService.GetByIdAsync(id);
            return this.ToActionResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AgencyResponse>>> CreateAgency([FromBody] CreateAgencyRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return this.ToActionResult(new ServiceResponse<AgencyResponse>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var result = await _agencyService.CreateAsync(dto);
            return this.ToCreatedAtActionResult(
                nameof(GetAgency),
                new { id = result.Data?.Id ?? Guid.Empty },
                result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AgencyResponse>>> UpdateAgency(Guid id, [FromBody] UpdateAgencyRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return this.ToActionResult(new ServiceResponse<AgencyResponse>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var result = await _agencyService.UpdateAsync(id, dto);
            return this.ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAgency(Guid id)
        {
            
            var result = await _agencyService.DeleteAsync(id);
            return this.ToActionResult(result, new { AgencyId = id });
        }
    }
}


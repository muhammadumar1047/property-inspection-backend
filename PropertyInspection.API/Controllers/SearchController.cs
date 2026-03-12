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
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<SearchResultGroupedDto>>> Search([FromQuery] string query , [FromQuery] Guid? agencyId)
        {
            var result = await _searchService.SearchAsync(query, agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("properties")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<SearchPropertyDto>>>> SearchProperty([FromQuery] string query)
        {
            if (!HttpContext.Items.ContainsKey("AgencyId") || HttpContext.Items["AgencyId"] == null)
            {
                return this.ToActionResult(new ServiceResponse<IReadOnlyList<SearchPropertyDto>>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            Guid agencyId = Guid.Parse(HttpContext.Items["AgencyId"]?.ToString() ?? Guid.Empty.ToString());

            var result = await _searchService.SearchPropertyAsync(query, agencyId);
            return this.ToActionResult(result, new { Count = result.Data?.Count ?? 0 });
        }
    }
}

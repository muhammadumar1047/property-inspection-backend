using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
          

            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Query cannot be empty.",
                    Data = false
                });
            }

            var results = await _searchService.SearchAsync(query, agencyId);

            if ((results.Properties == null || !results.Properties.Any()) &&
                (results.Inspections == null || !results.Inspections.Any()))
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No results found.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<SearchResultGroupedDto>
            {
                Success = true,
                Message = "Records retrieved successfully",
                Data = results
            });
        }

        [HttpGet("properties")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<SearchPropertyDto>>>> SearchProperty([FromQuery] string query)
        {
            if (!HttpContext.Items.ContainsKey("AgencyId") || HttpContext.Items["AgencyId"] == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "AgencyId is missing.",
                    Data = false
                });
            }

            Guid agencyId = Guid.Parse(HttpContext.Items["AgencyId"]?.ToString() ?? Guid.Empty.ToString());

            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Query cannot be empty.",
                    Data = false
                });
            }

            var results = await _searchService.SearchPropertyAsync(query, agencyId);

            if (results == null || !results.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No results found.",
                    Data = false
                });
            }

            var list = results.ToList();
            return Ok(new ApiResponse<IReadOnlyList<SearchPropertyDto>>
            {
                Success = true,
                Message = "Records retrieved successfully",
                Data = list,
                Meta = new { Count = list.Count }
            });
        }
    }
}

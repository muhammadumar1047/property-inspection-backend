using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<AnalyticsDto>>> GetAnalytics(Guid? agencyId)
        {
            var result = await _analyticsService.GetDashboardAnalyticsByAgencyAsync(agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<AnalyticsSummaryDto>>> GetSummary([FromQuery] AnalyticsFilterDto filter)
        {
            var result = await _analyticsService.GetAnalyticsSummaryAsync(filter);
            return this.ToActionResult(result);
        }

        [HttpGet("charts")]
        public async Task<ActionResult<ApiResponse<AnalyticsChartDto>>> GetCharts([FromQuery] AnalyticsFilterDto filter)
        {
            var result = await _analyticsService.GetAnalyticsChartsAsync(filter);
            return this.ToActionResult(result);
        }
    }
}

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
        [Permission("report.view")]
        public async Task<ActionResult<ApiResponse<AnalyticsDto>>> GetAnalytics()
        {
            if (!HttpContext.Items.ContainsKey("AgencyId") || HttpContext.Items["AgencyId"] == null)
            {
                return this.ToActionResult(new ServiceResponse<AnalyticsDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            Guid agencyId = Guid.Parse(HttpContext.Items["AgencyId"]?.ToString() ?? Guid.Empty.ToString());

            var result = await _analyticsService.GetDashboardAnalyticsByAgencyAsync(agencyId);
            return this.ToActionResult(result);
        }
    }
}

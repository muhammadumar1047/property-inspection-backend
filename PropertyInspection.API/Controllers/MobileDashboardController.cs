using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MobileDashboardController : ControllerBase
    {
        private readonly IMobileDashboardService _dashboardService;

        public MobileDashboardController(IMobileDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetDashboard(
            [FromQuery] Guid? agencyId,
            [FromQuery] Guid? inspectorId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var result = await _dashboardService.GetDashboardAsync(agencyId, inspectorId, startDate, endDate);
            return this.ToActionResult(result);
        }
    }
}

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
    public class ReportSyncController : ControllerBase
    {
        private readonly IReportSyncService _reportSyncService;

        public ReportSyncController(IReportSyncService reportSyncService)
        {
            _reportSyncService = reportSyncService;
        }

        [HttpPost("sync")]
        public async Task<ActionResult<ApiResponse<bool>>> SyncReport([FromBody] ReportSyncDto reportDto)
        {
            var result = await _reportSyncService.SyncReportAsync(reportDto);
            return this.ToActionResult(result);
        }
    }
}

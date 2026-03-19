using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/mobile/report-template")]
    [ApiController]
    [Authorize]
    public class MobileReportTemplateController : ControllerBase
    {
        private readonly IReportTemplateService _mobileReportTemplateService;

        public MobileReportTemplateController(IReportTemplateService mobileReportTemplateService)
        {
            _mobileReportTemplateService = mobileReportTemplateService;
        }

        [HttpGet("entry-exit")]
        public async Task<ActionResult<ApiResponse<ReportTemplateDto>>> GetEntryExitTemplate([FromQuery] Guid inspectionId)
        {
            var result = await _mobileReportTemplateService.GenerateEntryExitTemplateAsync(inspectionId);
            return this.ToActionResult(result);
        }

        [HttpGet("routine")]
        public async Task<ActionResult<ApiResponse<ReportTemplateDto>>> GetRoutineTemplate([FromQuery] Guid inspectionId)
        {
            var result = await _mobileReportTemplateService.GenerateRoutineTemplateAsync(inspectionId);
            return this.ToActionResult(result);
        }
    }
}

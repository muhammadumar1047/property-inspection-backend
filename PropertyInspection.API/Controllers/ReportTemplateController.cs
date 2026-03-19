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
    public class ReportTemplateController : ControllerBase
    {
        private readonly IReportTemplateService _reportTemplateService;

        public ReportTemplateController(IReportTemplateService reportTemplateService)
        {
            _reportTemplateService = reportTemplateService;
        }

        [HttpGet("GenerateReportTemplateForPCR")]
        public async Task<ActionResult<ApiResponse<ReportTemplateDto>>> GenerateReportTemplateForPCR(Guid inspectionId)
        {
            var result = await _reportTemplateService.GenerateEntryExitTemplateAsync(inspectionId);
            return this.ToActionResult(result);
        }

        [HttpGet("GenerateReportTemplateForRoutine")]
        public async Task<ActionResult<ApiResponse<ReportTemplateDto>>> GenerateReportTemplateForRoutine(Guid inspectionId)
        {
            var result = await _reportTemplateService.GenerateRoutineTemplateAsync(inspectionId);
            return this.ToActionResult(result);
        }
    }
}

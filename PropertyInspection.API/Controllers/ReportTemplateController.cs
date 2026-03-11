using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var reportTemplate = await _reportTemplateService.GenerateReportTemplateForPCR(inspectionId);
            if (reportTemplate == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Record not found.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<ReportTemplateDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = reportTemplate
            });
        }

        [HttpGet("GenerateReportTemplateForRoutine")]
        public async Task<ActionResult<ApiResponse<ReportTemplateDto>>> GenerateReportTemplateForRoutine(Guid inspectionId)
        {
            var reportTemplate = await _reportTemplateService.GenerateReportTemplateForRoutine(inspectionId);
            if (reportTemplate == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Record not found.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<ReportTemplateDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = reportTemplate
            });
        }
    }
}

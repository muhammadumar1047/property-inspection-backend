using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using Microsoft.Extensions.Configuration;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IConfiguration _configuration;

        public ReportController(IReportService reportService, IConfiguration configuration)
        {
            _reportService = reportService;
            _configuration = configuration;
        }

        [HttpGet("inspection/{inspectionId}")]
        public async Task<ActionResult<ApiResponse<ReportDto>>> GetReportByInspectionId(Guid inspectionId, Guid? agencyId)
        {
            var result = await _reportService.GetReportByInspectionIdAsync(inspectionId , agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("inspection/{inspectionId}/pdf")]
        public ActionResult GetReportPdf(Guid inspectionId)
        {
            var baseUrl =
                _configuration["Frontend:BaseUrl"] ??
                _configuration["FrontendBaseUrl"] ??
                Request.Headers["Origin"].ToString();

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return BadRequest("Frontend base URL is not configured.");
            }

            var trimmedBase = baseUrl.TrimEnd('/');
            var reportUrl = $"{trimmedBase}/inspections/{inspectionId}/report?pdf=1";
            var filename = $"inspection-report-{inspectionId}.pdf";
            var pdfUrl =
                $"{trimmedBase}/api/pdf?url={Uri.EscapeDataString(reportUrl)}&filename={Uri.EscapeDataString(filename)}";

            return Redirect(pdfUrl);
        }
    }
}

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
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("inspection/{inspectionId}")]
        public async Task<ActionResult<ApiResponse<ReportDto>>> GetReportByInspectionId(Guid inspectionId, Guid? agencyId)
        {
            var result = await _reportService.GetReportByInspectionIdAsync(inspectionId , agencyId);
            return this.ToActionResult(result);
        }
    }
}

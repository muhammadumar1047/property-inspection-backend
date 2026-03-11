using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var reportDto = await _reportService.GetReportByInspectionIdAsync(inspectionId , agencyId);

            if (reportDto == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"No report found for inspectionId {inspectionId}",
                    Data = false
                });
            }

            return Ok(new ApiResponse<ReportDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = reportDto
            });
        }
    }
}

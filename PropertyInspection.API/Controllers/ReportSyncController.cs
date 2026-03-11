using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            if (reportDto == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Report data is required.",
                    Data = false
                });
            }

            try
            {
                var success = await _reportSyncService.SyncReportAsync(reportDto);

                if (success)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "Record updated successfully",
                        Data = true
                    });
                }

                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to sync report.",
                    Data = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred.",
                    Data = ex.Message
                });
            }
        }
    }
}

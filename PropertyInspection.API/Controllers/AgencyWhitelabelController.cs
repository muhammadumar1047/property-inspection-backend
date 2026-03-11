using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgencyWhitelabelController : ControllerBase
    {
        private readonly IAgencyWhitelabelService _whitelabelService;

        public AgencyWhitelabelController(IAgencyWhitelabelService whitelabelService)
        {
            _whitelabelService = whitelabelService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<AgencyWhitelabelDto>>> GetAgencyWhitelabel(Guid? agencyId)
        {
            var whitelabel = await _whitelabelService.GetByAgencyIdAsync(agencyId);

            if (whitelabel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No whitelabel settings found for this agency.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<AgencyWhitelabelDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = whitelabel
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AgencyWhitelabelDto>>> GetWhitelabel(Guid id , Guid? agencyId)
        {
            var whitelabel = await _whitelabelService.GetByIdAsync(id , agencyId);

            if (whitelabel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Whitelabel with ID {id} not found.",
                    Data = false
                });
            }

            return Ok(new ApiResponse<AgencyWhitelabelDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = whitelabel
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AgencyWhitelabelDto>>> CreateWhitelabel(CreateAgencyWhitelabelDto createDto)
        {
            try
            {
                var whitelabel = await _whitelabelService.CreateAsync(createDto);
                return CreatedAtAction(
                    nameof(GetWhitelabel),
                    new { id = whitelabel.Id },
                    new ApiResponse<AgencyWhitelabelDto>
                    {
                        Success = true,
                        Message = "Entity created successfully",
                        Data = whitelabel
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error creating whitelabel: {ex.Message}",
                    Data = false
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AgencyWhitelabelDto>>> UpdateWhitelabel(Guid id, UpdateAgencyWhitelabelDto updateDto)
        {
            try
            {
                var whitelabel = await _whitelabelService.UpdateAsync(id, updateDto);

                if (whitelabel == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Whitelabel with ID {id} not found.",
                        Data = false
                    });
                }

                return Ok(new ApiResponse<AgencyWhitelabelDto>
                {
                    Success = true,
                    Message = "Record updated successfully",
                    Data = whitelabel
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error updating whitelabel: {ex.Message}",
                    Data = false
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteWhitelabel(Guid id , Guid? agencyId)
        {
            try
            {
                var success = await _whitelabelService.DeleteAsync(id , agencyId);

                if (!success)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Whitelabel with ID {id} not found.",
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Record deleted successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error deleting whitelabel: {ex.Message}",
                    Data = false
                });
            }
        }

        [HttpGet("branding")]
        public async Task<ActionResult<ApiResponse<WhitelabelBrandingDto>>> GetBranding(Guid? agencyId)
        {
            var branding = await _whitelabelService.GetBrandingAsync(agencyId);
            return Ok(new ApiResponse<WhitelabelBrandingDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = branding
            });
        }

        [HttpGet("report-settings")]
        public async Task<ActionResult<ApiResponse<WhitelabelReportSettingsDto>>> GetReportSettings(Guid? agencyId)
        {
            var reportSettings = await _whitelabelService.GetReportSettingsAsync(agencyId);
            return Ok(new ApiResponse<WhitelabelReportSettingsDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = reportSettings
            });
        }

        [HttpGet("default")]
        public async Task<ActionResult<ApiResponse<DefaultWhitelabelDto>>> GetDefaultBranding()
        {
            var defaultBranding = await _whitelabelService.GetDefaultBrandingAsync();
            return Ok(new ApiResponse<DefaultWhitelabelDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = defaultBranding
            });
        }

        [HttpGet("exists")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckWhitelabelExists(Guid? agencyId)
        {
            var exists = await _whitelabelService.ExistsAsync(agencyId);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = exists
            });
        }

        [HttpGet("public/branding")]
        public async Task<ActionResult<ApiResponse<WhitelabelBrandingDto>>> GetPublicBranding([FromQuery] string? domain = null)
        {
            try
            {
                var defaultBranding = await _whitelabelService.GetDefaultBrandingAsync();
                var branding = new WhitelabelBrandingDto
                {
                    AccentFontFamily = defaultBranding.FontFamily,
                    AddressColor = defaultBranding.AddressColor,
                    AgencyNameColor = defaultBranding.AgencyNameColor,
                    LogoUrl = defaultBranding.LogoUrl,
                    PrimaryColor = defaultBranding.PrimaryColor,
                    SecondaryColor = defaultBranding.SecondaryColor,
                    FontFamily = defaultBranding.FontFamily,
                    AccentColor = defaultBranding.AccentColor,
                };

                return Ok(new ApiResponse<WhitelabelBrandingDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = branding
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error retrieving public branding: {ex.Message}",
                    Data = false
                });
            }
        }
    }
}

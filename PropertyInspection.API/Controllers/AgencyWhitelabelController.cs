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
    public class AgencyWhitelabelController : ControllerBase
    {
        private readonly IAgencyWhitelabelService _whitelabelService;
        private readonly IS3StorageService _s3StorageService;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;

        public AgencyWhitelabelController(
            IAgencyWhitelabelService whitelabelService,
            IS3StorageService s3StorageService,
            ITenantAgencyResolver tenantAgencyResolver)
        {
            _whitelabelService = whitelabelService;
            _s3StorageService = s3StorageService;
            _tenantAgencyResolver = tenantAgencyResolver;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<AgencyWhitelabelResponse>>> GetAgencyWhitelabel(Guid? agencyId)
        {
            var result = await _whitelabelService.GetByAgencyIdAsync(agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AgencyWhitelabelResponse>>> GetWhitelabel(Guid id , Guid? agencyId)
        {
            var result = await _whitelabelService.GetByIdAsync(id , agencyId);
            return this.ToActionResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AgencyWhitelabelResponse>>> CreateWhitelabel(CreateAgencyWhitelabelRequest createDto)
        {
            var result = await _whitelabelService.CreateAsync(createDto);
            return this.ToCreatedAtActionResult(
                nameof(GetWhitelabel),
                new { id = result.Data?.Id ?? Guid.Empty },
                result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AgencyWhitelabelResponse>>> UpdateWhitelabel(Guid id, UpdateAgencyWhitelabelRequest updateDto)
        {
            var result = await _whitelabelService.UpdateAsync(id, updateDto);
            return this.ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteWhitelabel(Guid id , Guid? agencyId)
        {
            var result = await _whitelabelService.DeleteAsync(id , agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("branding")]
        public async Task<ActionResult<ApiResponse<WhitelabelBrandingDto>>> GetBranding(Guid? agencyId)
        {
            var result = await _whitelabelService.GetBrandingAsync(agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("report-settings")]
        public async Task<ActionResult<ApiResponse<WhitelabelReportSettingsDto>>> GetReportSettings(Guid? agencyId)
        {
            var result = await _whitelabelService.GetReportSettingsAsync(agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("default")]
        public async Task<ActionResult<ApiResponse<DefaultWhitelabelDto>>> GetDefaultBranding()
        {
            var result = await _whitelabelService.GetDefaultBrandingAsync();
            return this.ToActionResult(result);
        }

        [HttpGet("exists")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckWhitelabelExists(Guid? agencyId)
        {
            var result = await _whitelabelService.ExistsAsync(agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("public/branding")]
        public async Task<ActionResult<ApiResponse<WhitelabelBrandingDto>>> GetPublicBranding([FromQuery] string? domain = null)
        {
            var defaultBrandingResult = await _whitelabelService.GetDefaultBrandingAsync();
            if (!defaultBrandingResult.Success || defaultBrandingResult.Data == null)
            {
                return this.ToActionResult(new ServiceResponse<WhitelabelBrandingDto>
                {
                    Success = false,
                    Message = defaultBrandingResult.Message,
                    ErrorCode = defaultBrandingResult.ErrorCode
                });
            }

            var branding = new WhitelabelBrandingDto
            {
                AccentFontFamily = defaultBrandingResult.Data.FontFamily,
                AddressColor = defaultBrandingResult.Data.AddressColor,
                AgencyNameColor = defaultBrandingResult.Data.AgencyNameColor,
                LogoUrl = defaultBrandingResult.Data.LogoUrl,
                PrimaryColor = defaultBrandingResult.Data.PrimaryColor,
                SecondaryColor = defaultBrandingResult.Data.SecondaryColor,
                FontFamily = defaultBrandingResult.Data.FontFamily,
                AccentColor = defaultBrandingResult.Data.AccentColor,
            };

            return Ok(new ApiResponse<WhitelabelBrandingDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = branding
            });
        }

        [HttpPost("logo")]
        public async Task<ActionResult<ApiResponse<string>>> UploadLogo(IFormFile file)
        {
            try
            {
                // Read agencyId from FormData (injected by frontend interceptor when SuperAdmin impersonates an agency)
                var agencyIdFromForm = Request.Form["agencyId"].FirstOrDefault();
                Guid? requestedAgencyId = null;
                if (!string.IsNullOrWhiteSpace(agencyIdFromForm) && Guid.TryParse(agencyIdFromForm, out var parsed))
                {
                    requestedAgencyId = parsed;
                }
                var agencyId = _tenantAgencyResolver.ResolveAgencyId(requestedAgencyId);
                var url = await _s3StorageService.UploadLogoAsync(file, agencyId.ToString());
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Logo uploaded successfully",
                    Data = url
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}


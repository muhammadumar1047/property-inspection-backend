using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Threading.Tasks;

namespace PropertyInspection.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTemplatesController : ControllerBase
    {
        private readonly IEmailTemplateService _templateService;

        public EmailTemplatesController(IEmailTemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<EmailTemplateResponse>>>> GetTemplates(
            [FromQuery] string? search,
            [FromQuery] InspectionType? inspectionType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? agencyId = null)
        {
            var result = await _templateService.GetTemplatesAsync(search, inspectionType, page, pageSize, agencyId);
            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> GetById(Guid id, [FromQuery] Guid? agencyId = null)
        {
            var result = await _templateService.GetByIdAsync(id, agencyId);
            return this.ToActionResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> Create([FromBody] CreateEmailTemplateRequest request)
        {
            var result = await _templateService.CreateAsync(request);
            return this.ToCreatedAtActionResult(
                nameof(GetById),
                new { id = result.Data?.Id ?? Guid.Empty },
                result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<EmailTemplateResponse>>> Update(Guid id, [FromBody] UpdateEmailTemplateRequest request)
        {
            var result = await _templateService.UpdateAsync(id, request);
            return this.ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, [FromQuery] Guid? agencyId = null)
        {
            var result = await _templateService.DeleteAsync(id, agencyId);
            return this.ToActionResult(result);
        }

        [HttpPost("{id}/make-default")]
        public async Task<ActionResult<ApiResponse<bool>>> MakeDefault(Guid id, [FromBody] MakeDefaultRequest? request = null)
        {
            var result = await _templateService.MakeDefaultAsync(id, request?.AgencyId);
            return this.ToActionResult(result);
        }

        [HttpPost("send-test")]
        public async Task<ActionResult<ApiResponse<bool>>> SendTestEmail([FromBody] SendTestEmailRequest request)
        {
            var result = await _templateService.SendTestEmailAsync(request);
            return this.ToActionResult(result);
        }
    }
}

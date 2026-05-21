using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PropertyInspection.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuickSuggestionsController : ControllerBase
    {
        private readonly IQuickSuggestionService _suggestionService;

        public QuickSuggestionsController(IQuickSuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<QuickSuggestionResponse>>>> GetSuggestions(
            [FromQuery] QuickSuggestionType type,
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? agencyId = null)
        {
            var result = await _suggestionService.GetSuggestionsAsync(type, search, sortBy, page, pageSize, agencyId);
            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuickSuggestionResponse>>> GetById(Guid id, [FromQuery] Guid? agencyId = null)
        {
            var result = await _suggestionService.GetByIdAsync(id, agencyId);
            return this.ToActionResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QuickSuggestionResponse>>> Create([FromBody] CreateQuickSuggestionRequest request)
        {
            var result = await _suggestionService.CreateAsync(request);
            return this.ToCreatedAtActionResult(
                nameof(GetById),
                new { id = result.Data?.Id ?? Guid.Empty },
                result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<QuickSuggestionResponse>>> Update(Guid id, [FromBody] UpdateQuickSuggestionRequest request)
        {
            var result = await _suggestionService.UpdateAsync(id, request);
            return this.ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, [FromQuery] Guid? agencyId = null)
        {
            var result = await _suggestionService.DeleteAsync(id, agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("settings")]
        public async Task<ActionResult<ApiResponse<QuickSuggestionSettingsResponse>>> GetSettings([FromQuery] Guid? agencyId = null)
        {
            var result = await _suggestionService.GetSettingsAsync(agencyId);
            return this.ToActionResult(result);
        }

        [HttpPut("settings")]
        public async Task<ActionResult<ApiResponse<QuickSuggestionSettingsResponse>>> UpdateSettings([FromBody] UpdateQuickSuggestionSettingsRequest request)
        {
            var result = await _suggestionService.UpdateSettingsAsync(request);
            return this.ToActionResult(result);
        }

        [HttpPost("import/preview")]
        public async Task<ActionResult<ApiResponse<ImportPreviewResult>>> PreviewImport(
            [FromQuery] QuickSuggestionType type,
            IFormFile file,
            [FromQuery] Guid? agencyId = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponse<ImportPreviewResult>
                {
                    Success = false,
                    Message = "No file uploaded or file is empty."
                });
            }

            using var stream = file.OpenReadStream();
            var result = await _suggestionService.PreviewImportAsync(type, stream, file.FileName, agencyId);
            return this.ToActionResult(result);
        }

        [HttpPost("import/commit")]
        public async Task<ActionResult<ApiResponse<int>>> CommitImport([FromBody] CommitImportRequest request)
        {
            var result = await _suggestionService.CommitImportAsync(request);
            return this.ToActionResult(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(
            [FromQuery] QuickSuggestionType type,
            [FromQuery] Guid? agencyId = null)
        {
            var result = await _suggestionService.ExportToCsvAsync(type, agencyId);
            if (!result.Success || result.Data == null)
            {
                return BadRequest(result.Message);
            }

            var fileName = $"quicksuggestions_{type}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
            return File(result.Data, "text/csv", fileName);
        }
    }
}

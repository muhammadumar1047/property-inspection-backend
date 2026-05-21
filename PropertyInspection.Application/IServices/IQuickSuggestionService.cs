using PropertyInspection.Core.Enums;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface IQuickSuggestionService
    {
        Task<ServiceResponse<PagedResult<QuickSuggestionResponse>>> GetSuggestionsAsync(
            QuickSuggestionType type, string? search, string? sortBy, int page, int pageSize, Guid? agencyId = null);

        Task<ServiceResponse<QuickSuggestionResponse>> GetByIdAsync(Guid id, Guid? agencyId = null);

        Task<ServiceResponse<QuickSuggestionResponse>> CreateAsync(CreateQuickSuggestionRequest request);

        Task<ServiceResponse<QuickSuggestionResponse>> UpdateAsync(Guid id, UpdateQuickSuggestionRequest request);

        Task<ServiceResponse<bool>> DeleteAsync(Guid id, Guid? agencyId = null);

        // Settings Toggles
        Task<ServiceResponse<QuickSuggestionSettingsResponse>> GetSettingsAsync(Guid? agencyId = null);
        
        Task<ServiceResponse<QuickSuggestionSettingsResponse>> UpdateSettingsAsync(UpdateQuickSuggestionSettingsRequest request);

        // Bulk Importer / Exporter
        Task<ServiceResponse<ImportPreviewResult>> PreviewImportAsync(QuickSuggestionType type, Stream fileStream, string fileName, Guid? agencyId = null);

        Task<ServiceResponse<int>> CommitImportAsync(CommitImportRequest request);

        Task<ServiceResponse<byte[]>> ExportToCsvAsync(QuickSuggestionType type, Guid? agencyId = null);
    }
}

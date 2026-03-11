using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IAgencyWhitelabelService
    {
        // CRUD operations
        Task<AgencyWhitelabelDto?> GetByAgencyIdAsync(Guid? agencyId);
        Task<AgencyWhitelabelDto?> GetByIdAsync(Guid whitelabelId , Guid? agencyId);
        Task<AgencyWhitelabelDto> CreateAsync(CreateAgencyWhitelabelDto createDto);
        Task<AgencyWhitelabelDto?> UpdateAsync(Guid whitelabelId, UpdateAgencyWhitelabelDto updateDto);
        Task<bool> DeleteAsync(Guid whitelabelId, Guid? agencyId);

        // Branding operations
        Task<WhitelabelBrandingDto> GetBrandingAsync(Guid? agencyId);
        Task<WhitelabelReportSettingsDto> GetReportSettingsAsync(Guid? agencyId);
        Task<DefaultWhitelabelDto> GetDefaultBrandingAsync();

        // Validation operations
        Task<bool> ExistsAsync(Guid? agencyId);

        // Utility operations
        Task<IEnumerable<AgencyWhitelabelDto>> GetActiveWhitelabelsAsync();
        Task<string> SerializeContactDetailsAsync(WhitelabelContactDetailsDto contactDetails);
    }
}

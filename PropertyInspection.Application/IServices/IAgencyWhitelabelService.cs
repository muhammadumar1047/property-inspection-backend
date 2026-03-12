using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.IServices
{
    public interface IAgencyWhitelabelService
    {
        // CRUD operations
        Task<ServiceResponse<AgencyWhitelabelResponse>> GetByAgencyIdAsync(Guid? agencyId);
        Task<ServiceResponse<AgencyWhitelabelResponse>> GetByIdAsync(Guid whitelabelId , Guid? agencyId);
        Task<ServiceResponse<AgencyWhitelabelResponse>> CreateAsync(CreateAgencyWhitelabelRequest createDto);
        Task<ServiceResponse<AgencyWhitelabelResponse>> UpdateAsync(Guid whitelabelId, UpdateAgencyWhitelabelRequest updateDto);
        Task<ServiceResponse<bool>> DeleteAsync(Guid whitelabelId, Guid? agencyId);

        // Branding operations
        Task<ServiceResponse<WhitelabelBrandingDto>> GetBrandingAsync(Guid? agencyId);
        Task<ServiceResponse<WhitelabelReportSettingsDto>> GetReportSettingsAsync(Guid? agencyId);
        Task<ServiceResponse<DefaultWhitelabelDto>> GetDefaultBrandingAsync();

        // Validation operations
        Task<ServiceResponse<bool>> ExistsAsync(Guid? agencyId);

        // Utility operations
        Task<ServiceResponse<IReadOnlyList<AgencyWhitelabelResponse>>> GetActiveWhitelabelsAsync();
        Task<ServiceResponse<string>> SerializeContactDetailsAsync(WhitelabelContactDetailsDto contactDetails);
    }
}


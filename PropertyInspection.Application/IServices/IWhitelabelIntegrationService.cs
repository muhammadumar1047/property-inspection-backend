

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IWhitelabelIntegrationService
    {
        // Report generation with branding
        Task<PropertyInspection.Shared.ServiceResponse<WhitelabelReportSettingsDto>> GetReportBrandingAsync(Guid agencyId);

        // UI branding
        Task<PropertyInspection.Shared.ServiceResponse<WhitelabelBrandingDto>> GetUIBrandingAsync(Guid agencyId);
    }
}

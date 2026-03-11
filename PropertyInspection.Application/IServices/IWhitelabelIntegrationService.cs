

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IWhitelabelIntegrationService
    {
        // Report generation with branding
        Task<WhitelabelReportSettingsDto> GetReportBrandingAsync(Guid agencyId);

        // UI branding
        Task<WhitelabelBrandingDto> GetUIBrandingAsync(Guid agencyId);
    }
}

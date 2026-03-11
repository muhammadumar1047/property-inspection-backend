

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IAnalyticsService
    {
        Task<AnalyticsDto> GetDashboardAnalyticsByAgencyAsync(Guid agencyId);
    }
}

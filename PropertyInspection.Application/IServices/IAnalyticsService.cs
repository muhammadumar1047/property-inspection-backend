

using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.IServices
{
    public interface IAnalyticsService
    {
        Task<ServiceResponse<AnalyticsDto>> GetDashboardAnalyticsByAgencyAsync(Guid agencyId);
    }
}

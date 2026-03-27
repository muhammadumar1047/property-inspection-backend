using System;
using System.Threading.Tasks;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IAnalyticsService
    {
        Task<ServiceResponse<AnalyticsDto>> GetDashboardAnalyticsByAgencyAsync(Guid? agencyId);
        Task<ServiceResponse<AnalyticsSummaryDto>> GetAnalyticsSummaryAsync(AnalyticsFilterDto filter);
        Task<ServiceResponse<AnalyticsChartDto>> GetAnalyticsChartsAsync(AnalyticsFilterDto filter);
    }
}

using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IMobileDashboardService
    {
        Task<ServiceResponse<DashboardSummaryDto>> GetDashboardAsync(
            Guid? agencyId,
            Guid? inspectorId,
            DateTime? startDate,
            DateTime? endDate);
    }
}

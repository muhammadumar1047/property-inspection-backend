

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IReportSyncService
    {
        Task<PropertyInspection.Shared.ServiceResponse<bool>> SyncReportAsync(ReportSyncDto report);
    }
}

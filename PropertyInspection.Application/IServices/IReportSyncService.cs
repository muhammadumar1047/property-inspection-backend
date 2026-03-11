

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IReportSyncService
    {
        Task<bool> SyncReportAsync(ReportSyncDto report);
    }
}

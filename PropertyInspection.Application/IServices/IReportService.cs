using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IReportService
    {
        Task<ReportDto?> GetReportByInspectionIdAsync(Guid inspectionId , Guid? agencyId);
    }
}

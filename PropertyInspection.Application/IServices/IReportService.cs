using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IReportService
    {
        Task<PropertyInspection.Shared.ServiceResponse<ReportDto>> GetReportByInspectionIdAsync(Guid inspectionId , Guid? agencyId);
    }
}

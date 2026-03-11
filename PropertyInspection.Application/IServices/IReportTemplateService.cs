

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IReportTemplateService
    {
        Task<ReportTemplateDto?> GenerateReportTemplateForPCR(Guid inspectionId);
        Task<ReportTemplateDto?> GenerateReportTemplateForRoutine(Guid inspectionId);
    }
}

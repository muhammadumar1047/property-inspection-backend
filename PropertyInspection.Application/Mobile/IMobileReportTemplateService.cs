using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IMobileReportTemplateService
    {
        Task<ServiceResponse<ReportTemplateDto>> GenerateEntryExitTemplateAsync(Guid inspectionId);
        Task<ServiceResponse<ReportTemplateDto>> GenerateRoutineTemplateAsync(Guid inspectionId);
    }
}

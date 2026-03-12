

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IReportTemplateService
    {
        Task<PropertyInspection.Shared.ServiceResponse<ReportTemplateDto>> GenerateReportTemplateForPCR(Guid inspectionId);
        Task<PropertyInspection.Shared.ServiceResponse<ReportTemplateDto>> GenerateReportTemplateForRoutine(Guid inspectionId);
    }
}

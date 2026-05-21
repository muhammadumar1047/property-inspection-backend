using PropertyInspection.Core.Enums;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface IEmailTemplateService
    {
        Task<ServiceResponse<PagedResult<EmailTemplateResponse>>> GetTemplatesAsync(
            string? search, InspectionType? inspectionType, int page, int pageSize, Guid? agencyId = null);
            
        Task<ServiceResponse<EmailTemplateResponse>> GetByIdAsync(Guid id, Guid? agencyId = null);
        
        Task<ServiceResponse<EmailTemplateResponse>> CreateAsync(CreateEmailTemplateRequest request);
        
        Task<ServiceResponse<EmailTemplateResponse>> UpdateAsync(Guid id, UpdateEmailTemplateRequest request);
        
        Task<ServiceResponse<bool>> DeleteAsync(Guid id, Guid? agencyId = null);
        
        Task<ServiceResponse<bool>> MakeDefaultAsync(Guid id, Guid? agencyId = null);
        
        Task<ServiceResponse<bool>> SendTestEmailAsync(SendTestEmailRequest request);
    }
}

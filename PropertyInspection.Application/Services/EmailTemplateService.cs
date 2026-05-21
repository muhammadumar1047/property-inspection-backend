using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.Services;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public EmailTemplateService(
            IUnitOfWork unitOfWork,
            ITenantAgencyResolver tenantAgencyResolver,
            IMapper mapper,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<PagedResult<EmailTemplateResponse>>> GetTemplatesAsync(
            string? search, InspectionType? inspectionType, int page, int pageSize, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var (templates, totalCount) = await _unitOfWork.EmailTemplates.GetPagedAsync(
                    pageNumber: page,
                    pageSize: pageSize,
                    predicate: t => t.AgencyId == resolvedAgencyId &&
                                    !t.IsDeleted &&
                                    (!inspectionType.HasValue || t.InspectionType == inspectionType.Value) &&
                                    (string.IsNullOrWhiteSpace(search) || t.Name.Contains(search) || t.Subject.Contains(search)),
                    orderBy: q => q.OrderByDescending(t => t.IsDefault).ThenByDescending(t => t.CreatedAt)
                );

                var responseDtos = _mapper.Map<List<EmailTemplateResponse>>(templates);

                return new ServiceResponse<PagedResult<EmailTemplateResponse>>
                {
                    Success = true,
                    Message = "Templates retrieved successfully",
                    Data = new PagedResult<EmailTemplateResponse>
                    {
                        Data = responseDtos,
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = totalCount
                    }
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<PagedResult<EmailTemplateResponse>>
                {
                    Success = false,
                    Message = "Error retrieving templates",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<EmailTemplateResponse>> GetByIdAsync(Guid id, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var template = await _unitOfWork.EmailTemplates.FirstOrDefaultAsync(
                    t => t.Id == id && t.AgencyId == resolvedAgencyId && !t.IsDeleted);

                if (template == null)
                {
                    return new ServiceResponse<EmailTemplateResponse>
                    {
                        Success = false,
                        Message = "Template not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<EmailTemplateResponse>
                {
                    Success = true,
                    Message = "Template retrieved successfully",
                    Data = _mapper.Map<EmailTemplateResponse>(template)
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<EmailTemplateResponse>
                {
                    Success = false,
                    Message = "Error retrieving template",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<EmailTemplateResponse>> CreateAsync(CreateEmailTemplateRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new ServiceResponse<EmailTemplateResponse>
                    {
                        Success = false,
                        Message = "Invalid request payload",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(request.AgencyId);
                var template = _mapper.Map<EmailTemplate>(request);
                template.AgencyId = resolvedAgencyId;
                template.CreatedAt = DateTime.UtcNow;
                template.IsDeleted = false;

                if (template.IsDefault)
                {
                    // Reset other default templates of the same type for this agency
                    var currentDefaults = await _unitOfWork.EmailTemplates.GetAsync(
                        t => t.AgencyId == resolvedAgencyId && t.InspectionType == template.InspectionType && t.IsDefault && !t.IsDeleted);

                    foreach (var item in currentDefaults)
                    {
                        item.IsDefault = false;
                        await _unitOfWork.EmailTemplates.UpdateAsync(item);
                    }
                }

                await _unitOfWork.EmailTemplates.AddAsync(template);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<EmailTemplateResponse>
                {
                    Success = true,
                    Message = "Template created successfully",
                    Data = _mapper.Map<EmailTemplateResponse>(template)
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<EmailTemplateResponse>
                {
                    Success = false,
                    Message = "Error creating template",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<EmailTemplateResponse>> UpdateAsync(Guid id, UpdateEmailTemplateRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new ServiceResponse<EmailTemplateResponse>
                    {
                        Success = false,
                        Message = "Invalid request payload",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(request.AgencyId);
                var template = await _unitOfWork.EmailTemplates.FirstOrDefaultAsync(
                    t => t.Id == id && t.AgencyId == resolvedAgencyId && !t.IsDeleted);

                if (template == null)
                {
                    return new ServiceResponse<EmailTemplateResponse>
                    {
                        Success = false,
                        Message = "Template not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                _mapper.Map(request, template);
                template.UpdatedAt = DateTime.UtcNow;

                if (template.IsDefault)
                {
                    // Reset other default templates of the same type for this agency
                    var currentDefaults = await _unitOfWork.EmailTemplates.GetAsync(
                        t => t.AgencyId == resolvedAgencyId && t.InspectionType == template.InspectionType && t.IsDefault && t.Id != template.Id && !t.IsDeleted);

                    foreach (var item in currentDefaults)
                    {
                        item.IsDefault = false;
                        await _unitOfWork.EmailTemplates.UpdateAsync(item);
                    }
                }

                await _unitOfWork.EmailTemplates.UpdateAsync(template);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<EmailTemplateResponse>
                {
                    Success = true,
                    Message = "Template updated successfully",
                    Data = _mapper.Map<EmailTemplateResponse>(template)
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<EmailTemplateResponse>
                {
                    Success = false,
                    Message = "Error updating template",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid id, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var template = await _unitOfWork.EmailTemplates.FirstOrDefaultAsync(
                    t => t.Id == id && t.AgencyId == resolvedAgencyId && !t.IsDeleted);

                if (template == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Template not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                template.IsDeleted = true;
                template.DeletedAt = DateTime.UtcNow;
                await _unitOfWork.EmailTemplates.UpdateAsync(template);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Template deleted successfully",
                    Data = true
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Error deleting template",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> MakeDefaultAsync(Guid id, Guid? agencyId = null)
        {
            try
            {
                var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var template = await _unitOfWork.EmailTemplates.FirstOrDefaultAsync(
                    t => t.Id == id && t.AgencyId == resolvedAgencyId && !t.IsDeleted);

                if (template == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Template not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                // Reset all other templates of same type
                var currentDefaults = await _unitOfWork.EmailTemplates.GetAsync(
                    t => t.AgencyId == resolvedAgencyId && t.InspectionType == template.InspectionType && t.IsDefault && t.Id != template.Id && !t.IsDeleted);

                foreach (var item in currentDefaults)
                {
                    item.IsDefault = false;
                    await _unitOfWork.EmailTemplates.UpdateAsync(item);
                }

                template.IsDefault = true;
                template.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.EmailTemplates.UpdateAsync(template);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Template is now designated as the agency default",
                    Data = true
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Error modifying default configuration",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> SendTestEmailAsync(SendTestEmailRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.To))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid recipient or test configuration",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                await _emailService.SendAsync(request.To, request.Subject, request.Body);

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Test email dispatched successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to send test email: {ex.Message}",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }
    }
}

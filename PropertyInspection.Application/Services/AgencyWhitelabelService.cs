using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class AgencyWhitelabelService : IAgencyWhitelabelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;

        public AgencyWhitelabelService(IUnitOfWork unitOfWork, ITenantAgencyResolver tenantAgencyResolver, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;
        }

        // CRUD operations
        public async Task<ServiceResponse<AgencyWhitelabelResponse>> GetByAgencyIdAsync(Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var whitelabel = await _unitOfWork.AgencyWhitelabels
                    .FirstOrDefaultAsync(w => w.AgencyId == tenantAgencyId);

                if (whitelabel == null)
                {
                    return new ServiceResponse<AgencyWhitelabelResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<AgencyWhitelabelResponse>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<AgencyWhitelabelResponse>(whitelabel)
                };
            }
            catch
            {
                return new ServiceResponse<AgencyWhitelabelResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<AgencyWhitelabelResponse>> GetByIdAsync(Guid whitelabelId, Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var whitelabel = await _unitOfWork.AgencyWhitelabels
                    .FirstOrDefaultAsync(w => w.Id == whitelabelId);

                if (whitelabel == null)
                {
                    return new ServiceResponse<AgencyWhitelabelResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<AgencyWhitelabelResponse>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<AgencyWhitelabelResponse>(whitelabel)
                };
            }
            catch
            {
                return new ServiceResponse<AgencyWhitelabelResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }
        public async Task<ServiceResponse<AgencyWhitelabelResponse>> CreateAsync(CreateAgencyWhitelabelRequest createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return new ServiceResponse<AgencyWhitelabelResponse>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(createDto.AgencyId);
                var existsResponse = await ExistsAsync(tenantAgencyId);
                if (!existsResponse.Success)
                {
                    return new ServiceResponse<AgencyWhitelabelResponse>
                    {
                        Success = false,
                        Message = existsResponse.Message,
                        ErrorCode = existsResponse.ErrorCode
                    };
                }

                if (existsResponse.Data)
                {
                    return new ServiceResponse<AgencyWhitelabelResponse>
                    {
                        Success = false,
                        Message = "Record already exists",
                        ErrorCode = ServiceErrorCodes.Conflict
                    };
                }

                var whitelabel = _mapper.Map<AgencyWhitelabel>(createDto);
                whitelabel.AgencyId = tenantAgencyId;

                await _unitOfWork.AgencyWhitelabels.AddAsync(whitelabel);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<AgencyWhitelabelResponse>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = _mapper.Map<AgencyWhitelabelResponse>(whitelabel)
                };
            }
            catch
            {
                return new ServiceResponse<AgencyWhitelabelResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<AgencyWhitelabelResponse>> UpdateAsync(Guid whitelabelId, UpdateAgencyWhitelabelRequest updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    return new ServiceResponse<AgencyWhitelabelResponse>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(updateDto.AgencyId);

                var whitelabel = await _unitOfWork.AgencyWhitelabels
                    .FirstOrDefaultAsync(w =>
                        w.Id == whitelabelId &&
                        w.AgencyId == tenantAgencyId);
                if (whitelabel == null)
                {
                    return new ServiceResponse<AgencyWhitelabelResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                _mapper.Map(updateDto, whitelabel);

                await _unitOfWork.AgencyWhitelabels.UpdateAsync(whitelabel);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<AgencyWhitelabelResponse>
                {
                    Success = true,
                    Message = "Record updated successfully",
                    Data = _mapper.Map<AgencyWhitelabelResponse>(whitelabel)
                };
            }
            catch
            {
                return new ServiceResponse<AgencyWhitelabelResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid whitelabelId, Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var whitelabel = await _unitOfWork.AgencyWhitelabels
                    .FirstOrDefaultAsync(w =>
                        w.Id == whitelabelId &&
                        w.AgencyId == tenantAgencyId);
                if (whitelabel == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                _unitOfWork.AgencyWhitelabels.Remove(whitelabel);
                await _unitOfWork.CommitAsync();
                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record deleted successfully",
                    Data = true
                };
            }
            catch
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        // Branding operations
        public async Task<ServiceResponse<WhitelabelBrandingDto>> GetBrandingAsync(Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var whitelabel = await _unitOfWork.AgencyWhitelabels
                    .FirstOrDefaultAsync(w => w.AgencyId == tenantAgencyId);

                if (whitelabel == null)
                {
                    var defaultBranding = await GetDefaultBrandingAsync();
                    return new ServiceResponse<WhitelabelBrandingDto>
                    {
                        Success = true,
                        Message = "Record retrieved successfully",
                        Data = _mapper.Map<WhitelabelBrandingDto>(defaultBranding.Data)
                    };
                }

                return new ServiceResponse<WhitelabelBrandingDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<WhitelabelBrandingDto>(whitelabel)
                };
            }
            catch
            {
                return new ServiceResponse<WhitelabelBrandingDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<WhitelabelReportSettingsDto>> GetReportSettingsAsync(Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var whitelabel = await _unitOfWork.AgencyWhitelabels
                    .FirstOrDefaultAsync(w => w.AgencyId == tenantAgencyId);

                if (whitelabel == null)
                {
                    var defaultBranding = await GetDefaultBrandingAsync();
                    return new ServiceResponse<WhitelabelReportSettingsDto>
                    {
                        Success = true,
                        Message = "Record retrieved successfully",
                        Data = _mapper.Map<WhitelabelReportSettingsDto>(defaultBranding.Data)
                    };
                }

                return new ServiceResponse<WhitelabelReportSettingsDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<WhitelabelReportSettingsDto>(whitelabel)
                };
            }
            catch
            {
                return new ServiceResponse<WhitelabelReportSettingsDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<DefaultWhitelabelDto>> GetDefaultBrandingAsync()
        {
            return await Task.FromResult(new ServiceResponse<DefaultWhitelabelDto>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = new DefaultWhitelabelDto()
            });
        }

        public async Task<ServiceResponse<bool>> ExistsAsync(Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var exists = await _unitOfWork.AgencyWhitelabels
                    .CountAsync(w => w.AgencyId == tenantAgencyId) > 0;

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = exists
                };
            }
            catch
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        // Utility operations
        public async Task<ServiceResponse<IReadOnlyList<AgencyWhitelabelResponse>>> GetActiveWhitelabelsAsync()
        {
            try
            {
                var whitelabels = await _unitOfWork.AgencyWhitelabels.GetAsync(
                    predicate: w => EF.Property<bool>(w, "IsActive"),
                    orderBy: q => q.OrderBy(w => EF.Property<int>(w, "WhitelabelId")));
                return new ServiceResponse<IReadOnlyList<AgencyWhitelabelResponse>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<List<AgencyWhitelabelResponse>>(whitelabels)
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<AgencyWhitelabelResponse>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<string>> SerializeContactDetailsAsync(WhitelabelContactDetailsDto contactDetails)
        {
            try
            {
                return await Task.FromResult(new ServiceResponse<string>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = JsonSerializer.Serialize(contactDetails)
                });
            }
            catch
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        // Helper methods
    }
}


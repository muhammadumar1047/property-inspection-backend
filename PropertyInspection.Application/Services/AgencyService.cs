using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Authorization;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class AgencyService : IAgencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAgencyRoleProvisioningService _agencyRoleProvisioningService;
        private readonly ITenantContext _tenantContext;
        private readonly IUserAuthService _userAuthService;
        private readonly IMapper _mapper;


        public AgencyService(
            IUnitOfWork unitOfWork,
            IAgencyRoleProvisioningService agencyRoleProvisioningService,
            ITenantContext tenantContext,
            IUserAuthService userAuthService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _agencyRoleProvisioningService = agencyRoleProvisioningService;
            _tenantContext = tenantContext;
            _userAuthService = userAuthService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PagedResult<AgencyResponse>>> GetAllAsync(
            int pageNumber,
            int pageSize,
            Guid? countryId,
            Guid? stateId,
            string? name,
            string? suburb)
        {
            try
            {
                var (agencyEntities, totalCount) = await _unitOfWork.Agencies.GetPagedAsync(
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    predicate: a =>
                        !a.IsDeleted &&
                        (!countryId.HasValue || a.CountryId == countryId) &&
                        (!stateId.HasValue || a.StateId == stateId) &&
                        (string.IsNullOrWhiteSpace(name) || a.LegalBusinessName.Contains(name)) &&
                        (string.IsNullOrWhiteSpace(suburb) || (a.Suburb != null && a.Suburb.Contains(suburb))),
                    include: q => q
                        .Include(a => a.AgencyWhitelabel)
                        .Include(a => a.Country)
                        .Include(a => a.State)
                        .AsNoTracking(),
                    orderBy: q => q.OrderBy(a => a.LegalBusinessName)
                );

                var agencyDtos = _mapper.Map<List<AgencyResponse>>(agencyEntities);
                var result = new PagedResult<AgencyResponse>
                {
                    Data = agencyDtos,
                    Page = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return new ServiceResponse<PagedResult<AgencyResponse>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = result
                };
            }
            catch
            {
                return new ServiceResponse<PagedResult<AgencyResponse>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<AgencyResponse>> GetByIdAsync(Guid agencyId)
        {
            try
            {
                var agency = await _unitOfWork.Agencies.FirstOrDefaultAsync(
                    a => a.Id == agencyId,
                    include: query => query.Include(a => a.AgencyWhitelabel).AsNoTracking()
                );

                if (agency == null)
                {
                    return new ServiceResponse<AgencyResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<AgencyResponse>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<AgencyResponse>(agency)
                };
            }
            catch
            {
                return new ServiceResponse<AgencyResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<AgencyResponse>> CreateAsync(CreateAgencyRequest dto)
        {
            try
            {
                if (dto == null || !dto.CountryId.HasValue || !dto.StateId.HasValue || !dto.TimeZoneId.HasValue)
                {
                    return new ServiceResponse<AgencyResponse>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var agency = _mapper.Map<Agency>(dto);
                agency.CreatedAt = DateTime.UtcNow;
                agency.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Agencies.AddAsync(agency);
                await _unitOfWork.CommitAsync();

                var identityUser = new ApplicationUser
                {
                    UserName = dto.AdminEmail,
                    Email = dto.AdminEmail,
                    EmailConfirmed = true,
                    AgencyId = agency.Id
                };

                var identityResponse = await _userAuthService.CreateUserAsync(identityUser, dto.AdminPassword);
                if (!identityResponse.Success || identityResponse.Data == null)
                {
                    return new ServiceResponse<AgencyResponse>
                    {
                        Success = false,
                        Message = identityResponse.Message,
                        ErrorCode = identityResponse.ErrorCode ?? ServiceErrorCodes.ServerError
                    };
                }

                identityUser = identityResponse.Data;

                var adminUser = new User
                {
                    IdentityUserId = identityUser.Id,
                    AgencyId = agency.Id,
                    Email = dto.AdminEmail,
                    FirstName = dto.AdminFirstName,
                    LastName = dto.AdminLastName,
                    IsAgencyAdmin = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(adminUser);
                await _unitOfWork.CommitAsync();

                await _agencyRoleProvisioningService.EnsureDefaultRolesAsync(agency.Id);

                var agencyAdminRole = await _unitOfWork.Roles
                    .FirstOrDefaultAsync(r => r.AgencyId == agency.Id && r.Name == DefaultRoleNames.AgencyAdmin);

                if (agencyAdminRole == null)
                {
                    return new ServiceResponse<AgencyResponse>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = ServiceErrorCodes.ServerError
                    };
                }

                adminUser.UserRoles.Add(new UserRole { RoleId = agencyAdminRole.Id });
                await _unitOfWork.CommitAsync();

                var defaultWhitelabel = new AgencyWhitelabel
                {
                    AgencyId = agency.Id,
                    AgencyNameColor = "#1E40AF",
                    AddressColor = "#1E40AF",
                    AccentColor = "#10B981",
                    AccentFontFamily = "Arial, sans-serif",
                    LogoUrl = "/assets/default-logo.png",
                    PrimaryColor = "#1E40AF",
                    SecondaryColor = "#EF4444",
                    FontFamily = "Arial, sans-serif",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.AgencyWhitelabels.AddAsync(defaultWhitelabel);
                await _unitOfWork.CommitAsync();

                var agencyDto = _mapper.Map<AgencyResponse>(agency);
                agencyDto.AgencyWhitelabel = _mapper.Map<AgencyWhitelabelResponse>(defaultWhitelabel);

                return new ServiceResponse<AgencyResponse>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = agencyDto
                };
            }
            catch
            {
                return new ServiceResponse<AgencyResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
}

        public async Task<ServiceResponse<AgencyResponse>> UpdateAsync(Guid agencyId, UpdateAgencyRequest dto)
        {
            try
            {
                if (dto == null)
                {
                    return new ServiceResponse<AgencyResponse>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var existingAgency = await _unitOfWork.Agencies
                    .FirstOrDefaultAsync(a => a.Id == agencyId);

                if (existingAgency == null)
                {
                    return new ServiceResponse<AgencyResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                _mapper.Map(dto, existingAgency);
                existingAgency.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Agencies.UpdateAsync(existingAgency);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<AgencyResponse>
                {
                    Success = true,
                    Message = "Record updated successfully",
                    Data = _mapper.Map<AgencyResponse>(existingAgency)
                };
            }
            catch
            {
                return new ServiceResponse<AgencyResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }
        public async Task<ServiceResponse<bool>> DeleteAsync(Guid agencyId)
        {
            Guid deletedBy = Guid.Empty;

            try
            {
                var agency = await _unitOfWork.Agencies.FirstOrDefaultAsync(a => a.Id == agencyId);
                if (agency == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                await _unitOfWork.Agencies.DeleteAsync(agencyId, deletedBy);
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
    }
}


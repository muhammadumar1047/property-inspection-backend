using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Authorization;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Shared.DTOs;
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

        public async Task<(List<AgencyDto> Agencies, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            Guid? countryId,
            Guid? stateId,
            string? name,
            string? suburb)
        {
            // 1. Get paged entities
            var (agencyEntities, totalCount) = await _unitOfWork.Agencies.GetPagedAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                predicate: a =>
                    (!countryId.HasValue || a.CountryId == countryId) &&
                    (!stateId.HasValue || a.StateId == stateId) &&
                    (string.IsNullOrWhiteSpace(name) || a.LegalBusinessName.Contains(name)) &&
                    (string.IsNullOrWhiteSpace(suburb) || (a.Suburb != null && a.Suburb.Contains(suburb))),
                include : q => q.Include(a => a.AgencyWhitelabel).AsNoTracking(),
                orderBy: q => q.OrderBy(a => a.LegalBusinessName)
            );

            var agencyDtos = _mapper.Map<List<AgencyDto>>(agencyEntities);

            return (agencyDtos, totalCount);
        }

        public async Task<AgencyDto?> GetByIdAsync(Guid agencyId)
        {
            var agency = await _unitOfWork.Agencies.FirstOrDefaultAsync(
                a => a.Id == agencyId,
                include: query => query.Include(a => a.AgencyWhitelabel).AsNoTracking()
            );

            if (agency == null)
            {
                return null;
            }

            return _mapper.Map<AgencyDto>(agency);
        }

        public async Task<AgencyDto> CreateAsync(CreateAgencyDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (!dto.CountryId.HasValue) throw new ArgumentNullException(nameof(dto.CountryId));
            if (!dto.StateId.HasValue) throw new ArgumentNullException(nameof(dto.StateId));
            if (!dto.TimeZoneId.HasValue) throw new ArgumentNullException(nameof(dto.TimeZoneId));

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

            identityUser = await _userAuthService.CreateUserAsync(identityUser, dto.AdminPassword);

            // 3️⃣ Create Domain User linked to Identity user
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

            // 4️⃣ Ensure default roles for agency
            await _agencyRoleProvisioningService.EnsureDefaultRolesAsync(agency.Id);

            // 5️⃣ Fetch AgencyAdmin role
            var agencyAdminRole = await _unitOfWork.Roles
                .FirstOrDefaultAsync(r => r.AgencyId == agency.Id && r.Name == DefaultRoleNames.AgencyAdmin);

            if (agencyAdminRole == null)
                throw new Exception("Default role provisioning failed. Agency Admin role was not created.");

            adminUser.UserRoles.Add(new UserRole { RoleId = agencyAdminRole.Id });
            await _unitOfWork.CommitAsync();

            // 6️⃣ Create default whitelabel
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

            // 7️⃣ Map to DTO
            var agencyDto = _mapper.Map<AgencyDto>(agency);
            agencyDto.AgencyWhitelabel = _mapper.Map<AgencyWhitelabelDto>(defaultWhitelabel);
            return agencyDto;
}

        public async Task<AgencyDto?> UpdateAsync(Guid agencyId, UpdateAgencyDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existingAgency = await _unitOfWork.Agencies
                .FirstOrDefaultAsync(a => a.Id == agencyId);

            if (existingAgency == null)
                return null;

            _mapper.Map(dto, existingAgency);
            existingAgency.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _unitOfWork.Agencies.UpdateAsync(existingAgency);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<AgencyDto>(existingAgency);
        }
        public async Task<bool> DeleteAsync(Guid agencyId)
        {
           
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
               
                var agency = await _unitOfWork.Agencies.FirstOrDefaultAsync(a => a.Id == agencyId);
                if (agency == null)
                    return false;

                
                _unitOfWork.Agencies.Remove(agency);
                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

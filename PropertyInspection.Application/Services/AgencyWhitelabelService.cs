using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
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
        public async Task<AgencyWhitelabelDto?> GetByAgencyIdAsync(Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var whitelabel = await _unitOfWork.AgencyWhitelabels
                .FirstOrDefaultAsync(w => w.AgencyId == tenantAgencyId);
            return whitelabel != null ? _mapper.Map<AgencyWhitelabelDto>(whitelabel) : null;
        }

        public async Task<AgencyWhitelabelDto?> GetByIdAsync(Guid whitelabelId, Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
            var whitelabel = await _unitOfWork.AgencyWhitelabels
                .FirstOrDefaultAsync(w => w.Id == whitelabelId);
            return whitelabel != null ? _mapper.Map<AgencyWhitelabelDto>(whitelabel) : null;
        }
        public async Task<AgencyWhitelabelDto> CreateAsync(CreateAgencyWhitelabelDto createDto)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(createDto.AgencyId);
            // Check if agency already has whitelabel settings
            if (await ExistsAsync(tenantAgencyId))
            {
                throw new InvalidOperationException("Agency already has whitelabel settings. Use update instead.");
            }


            var whitelabel = _mapper.Map<AgencyWhitelabel>(createDto);
            whitelabel.AgencyId = tenantAgencyId;

            await _unitOfWork.AgencyWhitelabels.AddAsync(whitelabel);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<AgencyWhitelabelDto>(whitelabel);
        }

        public async Task<AgencyWhitelabelDto?> UpdateAsync(Guid whitelabelId, UpdateAgencyWhitelabelDto updateDto)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(updateDto.AgencyId);

            var whitelabel = await _unitOfWork.AgencyWhitelabels
                .FirstOrDefaultAsync(w =>
                    w.Id == whitelabelId &&
                    w.AgencyId == tenantAgencyId);
            if (whitelabel == null)
                return null;


            _mapper.Map(updateDto, whitelabel);

            await _unitOfWork.AgencyWhitelabels.UpdateAsync(whitelabel);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<AgencyWhitelabelDto>(whitelabel);
        }

        public async Task<bool> DeleteAsync(Guid whitelabelId, Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var whitelabel = await _unitOfWork.AgencyWhitelabels
                .FirstOrDefaultAsync(w =>
                    w.Id == whitelabelId &&
                    w.AgencyId == tenantAgencyId);
            if (whitelabel == null)
                return false;

            _unitOfWork.AgencyWhitelabels.Remove(whitelabel);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // Branding operations
        public async Task<WhitelabelBrandingDto> GetBrandingAsync(Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var whitelabel = await _unitOfWork.AgencyWhitelabels
                .FirstOrDefaultAsync(w => w.AgencyId == tenantAgencyId);

            if (whitelabel == null)
            {
                // Return default branding
                var defaultBranding = await GetDefaultBrandingAsync();
                return _mapper.Map<WhitelabelBrandingDto>(defaultBranding);
            }

            return _mapper.Map<WhitelabelBrandingDto>(whitelabel);
        }

        public async Task<WhitelabelReportSettingsDto> GetReportSettingsAsync(Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var whitelabel = await _unitOfWork.AgencyWhitelabels
                .FirstOrDefaultAsync(w => w.AgencyId == tenantAgencyId);

            if (whitelabel == null)
            {
                // Return default report settings
                var defaultBranding = await GetDefaultBrandingAsync();
                return _mapper.Map<WhitelabelReportSettingsDto>(defaultBranding);
            }

            return _mapper.Map<WhitelabelReportSettingsDto>(whitelabel);
        }

        public async Task<DefaultWhitelabelDto> GetDefaultBrandingAsync()
        {
            return await Task.FromResult(new DefaultWhitelabelDto());
        }

        public async Task<bool> ExistsAsync(Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            return await _unitOfWork.AgencyWhitelabels
                .CountAsync(w => w.AgencyId == tenantAgencyId) > 0;
        }

        // Utility operations
        public async Task<IEnumerable<AgencyWhitelabelDto>> GetActiveWhitelabelsAsync()
        {
            var whitelabels = await _unitOfWork.AgencyWhitelabels.GetAsync(
                predicate: w => EF.Property<bool>(w, "IsActive"),
                orderBy: q => q.OrderBy(w => EF.Property<int>(w, "WhitelabelId")));
            return _mapper.Map<IEnumerable<AgencyWhitelabelDto>>(whitelabels);
        }

        public async Task<string> SerializeContactDetailsAsync(WhitelabelContactDetailsDto contactDetails)
        {
            return await Task.FromResult(JsonSerializer.Serialize(contactDetails));
        }

        // Helper methods
    }
}

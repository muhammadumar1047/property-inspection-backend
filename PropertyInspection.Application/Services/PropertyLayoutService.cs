using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class PropertyLayoutService : IPropertyLayoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;

        public PropertyLayoutService(IUnitOfWork unitOfWork, ITenantAgencyResolver tenantAgencyResolver, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<PropertyLayoutDto>,int TotalCount)> GetAllByAgencyAsync(
            Guid? agencyId,
             int pageNumber = 1,
             int pageSize = 10
        )
        {

            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            // Use generic GetAsync with predicate
            var (layouts, totalCount) = await _unitOfWork.PropertyLayout.GetPagedAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                predicate: l => l.AgencyId == tenantAgencyId,
                include: q => q.Include(l => l.Areas)
                                  .ThenInclude(p => p.Items),
                orderBy: q => q.OrderBy(l => l.DisplayOrder)
            );

            var result = _mapper.Map<List<PropertyLayoutDto>>(layouts);

            return (result, totalCount);
        }

        public async Task<PropertyLayoutDto?> GetByIdAsync(Guid layoutId, Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
            var layout = await _unitOfWork.PropertyLayout.GetAsync(
                predicate: l => l.Id == layoutId && l.AgencyId == tenantAgencyId,
                include: q => q
                    .Include(l => l.Areas)             // include related areas
                    .ThenInclude(a => a.Items)        // include items in each area
            );

            var entity = layout.FirstOrDefault();
            if (entity == null) return null;

            return _mapper.Map<PropertyLayoutDto>(entity);
        }

        public async Task<PropertyLayoutDto> CreateAsync(CreatePropertyLayoutDto layoutDto)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(layoutDto.AgencyId);

            var layout = _mapper.Map<PropertyLayout>(layoutDto);
            layout.AgencyId = tenantAgencyId;
            SetLayoutTimestamps(layout, DateTime.UtcNow, isNew: true);

            await _unitOfWork.PropertyLayout.AddAsync(layout);
            await _unitOfWork.CommitAsync();

            return await GetByIdAsync(layout.Id, layout.AgencyId)
                   ?? throw new Exception("Error creating layout");
        }


        public async Task<PropertyLayoutDto> UpdateAsync(PropertyLayoutDto layoutDto)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(layoutDto.AgencyId);

            var layout = (await _unitOfWork.PropertyLayout.GetAsync(
                l => l.Id == layoutDto.Id && l.AgencyId == tenantAgencyId)).FirstOrDefault();

            if (layout == null) throw new Exception("Layout not found");

            _mapper.Map(layoutDto, layout);
            layout.UpdatedAt = DateTime.UtcNow;

            layout.Areas.Clear();
            if (layoutDto.LayoutArea != null)
            {
                var mappedAreas = _mapper.Map<List<LayoutArea>>(layoutDto.LayoutArea);
                foreach (var area in mappedAreas)
                {
                    layout.Areas.Add(area);
                }
            }
            SetAreaTimestamps(layout.Areas, DateTime.UtcNow, isNew: true);

            await _unitOfWork.PropertyLayout.UpdateAsync(layout);
            await _unitOfWork.CommitAsync();

            return await GetByIdAsync(layout.Id, layout.AgencyId)
                   ?? throw new Exception("Error updating layout");
        }

        public async Task<bool> DeleteAsync(Guid layoutId, Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var layout = (await _unitOfWork.PropertyLayout.GetAsync(
                l => l.Id == layoutId && l.AgencyId == tenantAgencyId)).FirstOrDefault();

            if (layout == null) return false;

            await _unitOfWork.PropertyLayout.DeleteAsync(layout.Id, Guid.NewGuid());
            await _unitOfWork.CommitAsync();

            return true;
        }

        private static void SetLayoutTimestamps(PropertyLayout layout, DateTime nowUtc, bool isNew)
        {
            if (isNew)
            {
                layout.CreatedAt = nowUtc;
            }
            layout.UpdatedAt = nowUtc;

            SetAreaTimestamps(layout.Areas, nowUtc, isNew);
        }

        private static void SetAreaTimestamps(IEnumerable<LayoutArea> areas, DateTime nowUtc, bool isNew)
        {
            foreach (var area in areas)
            {
                if (isNew)
                {
                    area.CreatedAt = nowUtc;
                }
                area.UpdatedAt = nowUtc;

                foreach (var item in area.Items)
                {
                    if (isNew)
                    {
                        item.CreatedAt = nowUtc;
                    }
                    item.UpdatedAt = nowUtc;
                }
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using AutoMapper;

namespace PropertyInspection.Application.Services
{
    public class InspectionService : IInspectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;

        public InspectionService(IUnitOfWork unitOfWork, ITenantAgencyResolver tenantAgencyResolver, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;

        }

        public async Task<(IEnumerable<InspectionDto> Inspections, int TotalCount)> GetAllInspectionsAsync(
            Guid? agencyId,
            Guid inspectionId,
            int pageNumber = 1,
            int pageSize = 10,
            InspectionType? inspectionType = null,
            InspectionStatus? inspectionStatus = null,
            Guid? inspectorId = null,
            string? suburb = null,
            DateTime? inspectionDate = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? searchProperty = null)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var (inspections, totalCount) = await _unitOfWork.Inspections.GetPagedAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                predicate: i =>
                     i.AgencyId == tenantAgencyId &&
                    (!inspectionType.HasValue || i.InspectionType == inspectionType) &&
                    (!inspectionStatus.HasValue || i.InspectionStatus == inspectionStatus) &&
                    (!inspectorId.HasValue || i.InspectorId == inspectorId.Value) &&
                    (string.IsNullOrWhiteSpace(suburb) || i.Property.CityOrSuburb.Contains(suburb)) &&
                    (!inspectionDate.HasValue || i.InspectionDate.Date == inspectionDate.Value.Date) &&
                    (!startDate.HasValue || i.InspectionDate >= startDate.Value) &&
                    (!endDate.HasValue || i.InspectionDate <= endDate.Value) &&
                    (string.IsNullOrWhiteSpace(searchProperty) || i.Property.Address1.Contains(searchProperty)),
                include: q => q
                    .Include(i => i.Property)
                    .Include(i => i.Inspector)
                    .Include(i => i.LandlordSnapshots)
                    .Include(i => i.TenancySnapshots),
                orderBy: q => q.OrderByDescending(i => i.InspectionDate));

            return (_mapper.Map<List<InspectionDto>>(inspections), totalCount);
        }

        public async Task<InspectionDto?> GetByIdAsync(Guid inspectionId, Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var inspection = await _unitOfWork.Inspections.FirstOrDefaultAsync(
                i => i.Id == inspectionId && i.AgencyId == tenantAgencyId,
                include: q => q
                    .Include(i => i.Property)
                    .Include(i => i.Inspector)
                    .Include(i => i.LandlordSnapshots)
                    .Include(i => i.TenancySnapshots));

            return inspection == null ? null : _mapper.Map<InspectionDto>(inspection);
        }

        public async Task<InspectionDto> CreateAsync(CreateInspectionDto inspectionDto)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(inspectionDto.AgencyId);

            inspectionDto.AgencyId = tenantAgencyId;

            var entity = _mapper.Map<Inspection>(inspectionDto);
            entity.AgencyId = tenantAgencyId;


            //var entity = MapToEntity(inspectionDto);
            await _unitOfWork.Inspections.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<InspectionDto>(entity);
        }

        public async Task<InspectionDto?> UpdateAsync(InspectionDto inspectionDto)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(inspectionDto.AgencyId);

            var existing = await _unitOfWork.Inspections.FirstOrDefaultAsync(
                i => i.Id == inspectionDto.Id && i.AgencyId == tenantAgencyId,
                include: q => q
                    .Include(i => i.LandlordSnapshots)
                    .Include(i => i.TenancySnapshots));

            if (existing == null)
                return null;

            _mapper.Map(inspectionDto, existing);

            await _unitOfWork.Inspections.UpdateAsync(existing);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<InspectionDto>(existing);
        }
            
        public async Task<List<InspectionDto>> GetAllByPropertyAsync(Guid propertyId, Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var inspections = await _unitOfWork.Inspections.GetAsync(
                predicate: i => i.PropertyId == propertyId && i.AgencyId == tenantAgencyId,
                include: q => q
                    .Include(i => i.Property)
                    .Include(i => i.Inspector)
                    .Include(i => i.LandlordSnapshots)
                    .Include(i => i.TenancySnapshots),
                orderBy: q => q.OrderByDescending(i => i.InspectionDate));

            return _mapper.Map<List<InspectionDto>>(inspections);
        }

        public async Task<bool> DeleteAsync(Guid inspectionId , Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var deleted = await _unitOfWork.Inspections.DeleteWhereAsync(
                i => i.Id == inspectionId && i.AgencyId == tenantAgencyId
            );
            if (!deleted)
                return false;

            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteLandlordSnapshotAsync(Guid landlordSnapshotId , Guid? agencyId)
        {
            var deleted = await _unitOfWork.LandlordSnapshots.DeleteWhereAsync(
                ls => ls.Id == landlordSnapshotId && ls.Inspection.AgencyId == _tenantAgencyResolver.ResolveAgencyId(agencyId)
            );
            if (!deleted)
                return false;

            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteTenancySnapshotAsync(Guid tenancySnapshotId, Guid? agencyId)
        {
            var deleted = await _unitOfWork.TenancySnapshots.DeleteWhereAsync(
                ts => ts.Id == tenancySnapshotId && ts.Inspection.AgencyId == _tenantAgencyResolver.ResolveAgencyId(agencyId)
            );
            if (!deleted)
                return false;

            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}

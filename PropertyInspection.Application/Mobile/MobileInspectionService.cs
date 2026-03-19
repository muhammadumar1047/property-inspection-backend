using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.Services
{
    public class MobileInspectionService : IMobileInspectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;

        public MobileInspectionService(
            IUnitOfWork unitOfWork,
            ITenantAgencyResolver tenantAgencyResolver,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PagedResult<InspectionResponse>>> GetMobileInspectionsAsync(
            Guid? agencyId,
            int pageNumber,
            int pageSize,
            Guid? propertyId,
            InspectionType? inspectionType,
            InspectionStatus? inspectionStatus,
            DateTime? startDate,
            DateTime? endDate,
            string? propertySearch)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var trimmedSearch = string.IsNullOrWhiteSpace(propertySearch) ? null : propertySearch.Trim();

                var start = startDate?.Date;
                var end = endDate.HasValue
                    ? endDate.Value.Date.AddDays(1).AddTicks(-1)
                    : (DateTime?)null;

                if (start.HasValue && end.HasValue && start.Value > end.Value)
                {
                    return new ServiceResponse<PagedResult<InspectionResponse>>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var searchPattern = trimmedSearch == null ? null : $"%{trimmedSearch}%";

                var (inspections, totalCount) = await _unitOfWork.Inspections.GetPagedAsync(
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    predicate: i =>
                        i.AgencyId == tenantAgencyId &&
                        (!propertyId.HasValue || i.PropertyId == propertyId) &&
                        (!inspectionType.HasValue || i.InspectionType == inspectionType) &&
                        (!inspectionStatus.HasValue || i.InspectionStatus == inspectionStatus) &&
                        (!start.HasValue || i.InspectionDate >= start.Value) &&
                        (!end.HasValue || i.InspectionDate <= end.Value) &&
                        (searchPattern == null
                            || EF.Functions.ILike(i.Property.Name ?? string.Empty, searchPattern)
                            || EF.Functions.ILike(i.Property.Address1 ?? string.Empty, searchPattern)
                            || EF.Functions.ILike(i.Property.Address2 ?? string.Empty, searchPattern)
                            || EF.Functions.ILike(i.Property.CityOrSuburb ?? string.Empty, searchPattern)
                            || EF.Functions.ILike(i.Property.Postcode ?? string.Empty, searchPattern)),
                    include: q => q
                        .AsSplitQuery()
                        .Include(i => i.Property)
                            .ThenInclude(p => p.Tenancies)
                               .ThenInclude(t => t.Tenants)
                        .Include(i => i.Property)
                            .ThenInclude(p => p.Landlords)
                        .Include(i => i.Inspector),
                        //.Include(i => i.LandlordSnapshots)
                        //.Include(i => i.TenancySnapshots),
                    orderBy: q => q.OrderByDescending(i => i.InspectionDate));

                var result = new PagedResult<InspectionResponse>
                {
                    Data = _mapper.Map<List<InspectionResponse>>(inspections),
                    Page = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return new ServiceResponse<PagedResult<InspectionResponse>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = result
                };
            }
            catch
            {
                return new ServiceResponse<PagedResult<InspectionResponse>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }
    }
}

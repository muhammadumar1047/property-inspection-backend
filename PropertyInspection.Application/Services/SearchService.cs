using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using AutoMapper;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;


        public SearchService(IUnitOfWork unitOfWork, ITenantAgencyResolver tenantAgencyResolver, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;

        }

        public async Task<ServiceResponse<SearchResultGroupedDto>> SearchAsync(string query, Guid? agencyId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return new ServiceResponse<SearchResultGroupedDto>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var properties = await _unitOfWork.Properties.GetAsync(
                    predicate: p =>
                        p.AgencyId == tenantAgencyId &&
                        (string.IsNullOrWhiteSpace(query) ||
                         (p.Address1 != null && p.Address1.Contains(query)) ||
                         (p.Address2 != null && p.Address2.Contains(query)) ||
                         p.CityOrSuburb.Contains(query)),
                    include: q => q
                        .Include(p => p.Tenancies)
                        .ThenInclude(t => t.Tenants)
                        .Include(p => p.Landlords));

                var inspections = await _unitOfWork.Inspections.GetAsync(
                    predicate: i =>
                        i.AgencyId == agencyId &&
                        (string.IsNullOrWhiteSpace(query) ||
                         (i.Property.Address1 != null && i.Property.Address1.Contains(query)) ||
                         (i.Property.Address2 != null && i.Property.Address2.Contains(query)) ||
                         i.Property.CityOrSuburb.Contains(query)),
                    include: q => q
                        .Include(i => i.Property)
                            .ThenInclude(p => p.Tenancies)
                        .Include(i => i.Property)
                            .ThenInclude(p => p.Landlords)
                        .Include(i => i.InspectionType));

                var propertyResults = _mapper.Map<List<SearchResultDto>>(properties)
                    .OrderBy(r => r.Address1)
                    .ToList();

                var inspectionResults = _mapper.Map<List<SearchResultDto>>(inspections)
                    .OrderBy(r => r.Address1)
                    .ToList();

                var grouped = new SearchResultGroupedDto
                {
                    Properties = propertyResults,
                    Inspections = inspectionResults
                };

                if ((grouped.Properties == null || grouped.Properties.Count == 0) &&
                    (grouped.Inspections == null || grouped.Inspections.Count == 0))
                {
                    return new ServiceResponse<SearchResultGroupedDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<SearchResultGroupedDto>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = grouped
                };
            }
            catch
            {
                return new ServiceResponse<SearchResultGroupedDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<IReadOnlyList<SearchPropertyDto>>> SearchPropertyAsync(string query, Guid? agencyId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return new ServiceResponse<IReadOnlyList<SearchPropertyDto>>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var properties = await _unitOfWork.Properties.GetAsync(
                    predicate: p =>
                        p.AgencyId == tenantAgencyId &&
                        (string.IsNullOrWhiteSpace(query) ||
                         (p.Address1 != null && p.Address1.Contains(query)) ||
                         (p.Address2 != null && p.Address2.Contains(query)) ||
                         p.CityOrSuburb.Contains(query)),
                    orderBy: q => q.OrderBy(p => p.Address1));

                var results = _mapper.Map<List<SearchPropertyDto>>(properties)
                    .OrderBy(r => r.Address)
                    .ToList();

                if (results.Count == 0)
                {
                    return new ServiceResponse<IReadOnlyList<SearchPropertyDto>>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<IReadOnlyList<SearchPropertyDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = results
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<SearchPropertyDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using AutoMapper;
using PropertyInspection.Shared;

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

        public async Task<ServiceResponse<PagedResult<InspectionResponse>>> GetAllInspectionsAsync(
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
            try
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

        public async Task<ServiceResponse<InspectionResponse>> GetByIdAsync(Guid inspectionId, Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var inspection = await _unitOfWork.Inspections.FirstOrDefaultAsync(
                    i => i.Id == inspectionId && i.AgencyId == tenantAgencyId,
                    include: q => q
                        .Include(i => i.Property)
                        .Include(i => i.Inspector)
                        .Include(i => i.LandlordSnapshots)
                        .Include(i => i.TenancySnapshots));

                if (inspection == null)
                {
                    return new ServiceResponse<InspectionResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<InspectionResponse>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<InspectionResponse>(inspection)
                };
            }
            catch
            {
                return new ServiceResponse<InspectionResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<InspectionResponse>> CreateAsync(CreateInspectionRequest inspectionDto)
        {
            try
            {
                if (inspectionDto == null)
                {
                    return new ServiceResponse<InspectionResponse>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(inspectionDto.AgencyId);

                inspectionDto.AgencyId = tenantAgencyId;

                var entity = _mapper.Map<Inspection>(inspectionDto);
                entity.AgencyId = tenantAgencyId;

                await _unitOfWork.Inspections.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<InspectionResponse>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = _mapper.Map<InspectionResponse>(entity)
                };
            }
            catch
            {
                return new ServiceResponse<InspectionResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(Guid inspectionId, UpdateInspectionRequest inspectionRequest)
        {
            try
            {
                if (inspectionRequest == null || inspectionId != inspectionRequest.Id)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(inspectionRequest.AgencyId);

                var existing = await _unitOfWork.Inspections.FirstOrDefaultAsync(
                    i => i.Id == inspectionRequest.Id && i.AgencyId == tenantAgencyId,
                    include: q => q
                        .Include(i => i.LandlordSnapshots)
                        .Include(i => i.TenancySnapshots));

                if (existing == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                _mapper.Map(inspectionRequest, existing);

                await _unitOfWork.Inspections.UpdateAsync(existing);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record updated successfully",
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
            
        public async Task<ServiceResponse<IReadOnlyList<InspectionResponse>>> GetAllByPropertyAsync(Guid propertyId, Guid? agencyId)
        {
            try
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

                return new ServiceResponse<IReadOnlyList<InspectionResponse>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<List<InspectionResponse>>(inspections)
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<InspectionResponse>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid inspectionId , Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var deleted = await _unitOfWork.Inspections.DeleteWhereAsync(
                    i => i.Id == inspectionId && i.AgencyId == tenantAgencyId
                );
                if (!deleted)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

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

        public async Task<ServiceResponse<bool>> DeleteLandlordSnapshotAsync(Guid landlordSnapshotId , Guid? agencyId)
        {
            try
            {
                var deleted = await _unitOfWork.LandlordSnapshots.DeleteWhereAsync(
                    ls => ls.Id == landlordSnapshotId && ls.Inspection.AgencyId == _tenantAgencyResolver.ResolveAgencyId(agencyId)
                );
                if (!deleted)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

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

        public async Task<ServiceResponse<bool>> DeleteTenancySnapshotAsync(Guid tenancySnapshotId, Guid? agencyId)
        {
            try
            {
                var deleted = await _unitOfWork.TenancySnapshots.DeleteWhereAsync(
                    ts => ts.Id == tenancySnapshotId && ts.Inspection.AgencyId == _tenantAgencyResolver.ResolveAgencyId(agencyId)
                );
                if (!deleted)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

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


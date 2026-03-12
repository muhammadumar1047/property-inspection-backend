using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;
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

        public async Task<ServiceResponse<PagedResult<PropertyLayoutResponse>>> GetAllByAgencyAsync(
            Guid? agencyId,
             int pageNumber = 1,
             int pageSize = 10
        )
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var (layouts, totalCount) = await _unitOfWork.PropertyLayout.GetPagedAsync(
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    predicate: l => l.AgencyId == tenantAgencyId,
                    include: q => q.Include(l => l.Areas)
                                      .ThenInclude(p => p.Items),
                    orderBy: q => q.OrderBy(l => l.DisplayOrder)
                );

                var result = _mapper.Map<List<PropertyLayoutResponse>>(layouts);
                var paged = new PagedResult<PropertyLayoutResponse>
                {
                    Data = result,
                    Page = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return new ServiceResponse<PagedResult<PropertyLayoutResponse>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = paged
                };
            }
            catch
            {
                return new ServiceResponse<PagedResult<PropertyLayoutResponse>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<PropertyLayoutResponse>> GetByIdAsync(Guid layoutId, Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var layout = await _unitOfWork.PropertyLayout.GetAsync(
                    predicate: l => l.Id == layoutId && l.AgencyId == tenantAgencyId,
                    include: q => q
                        .Include(l => l.Areas)
                        .ThenInclude(a => a.Items)
                );

                var entity = layout.FirstOrDefault();
                if (entity == null)
                {
                    return new ServiceResponse<PropertyLayoutResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<PropertyLayoutResponse>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<PropertyLayoutResponse>(entity)
                };
            }
            catch
            {
                return new ServiceResponse<PropertyLayoutResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<PropertyLayoutResponse>> CreateAsync(CreatePropertyLayoutRequest layoutDto)
        {
            try
            {
                if (layoutDto == null)
                {
                    return new ServiceResponse<PropertyLayoutResponse>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(layoutDto.AgencyId);

                var layout = _mapper.Map<PropertyLayout>(layoutDto);
                layout.AgencyId = tenantAgencyId;
                SetLayoutTimestamps(layout, DateTime.UtcNow, isNew: true);

                await _unitOfWork.PropertyLayout.AddAsync(layout);
                await _unitOfWork.CommitAsync();

                var created = await GetByIdAsync(layout.Id, layout.AgencyId);
                if (!created.Success || created.Data == null)
                {
                    return new ServiceResponse<PropertyLayoutResponse>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = ServiceErrorCodes.ServerError
                    };
                }

                return new ServiceResponse<PropertyLayoutResponse>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = created.Data
                };
            }
            catch
            {
                return new ServiceResponse<PropertyLayoutResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }


        public async Task<ServiceResponse<bool>> UpdateAsync(Guid layoutId, UpdatePropertyLayoutRequest layoutRequest)
        {
            try
            {
                if (layoutRequest == null || layoutId != layoutRequest.Id)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(layoutRequest.AgencyId);

                var layout = (await _unitOfWork.PropertyLayout.GetAsync(
                    l => l.Id == layoutRequest.Id && l.AgencyId == tenantAgencyId)).FirstOrDefault();

                if (layout == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                _mapper.Map(layoutRequest, layout);
                layout.UpdatedAt = DateTime.UtcNow;

                layout.Areas.Clear();
                if (layoutRequest.LayoutArea != null)
                {
                    var mappedAreas = _mapper.Map<List<LayoutArea>>(layoutRequest.LayoutArea);
                    foreach (var area in mappedAreas)
                    {
                        layout.Areas.Add(area);
                    }
                }
                SetAreaTimestamps(layout.Areas, DateTime.UtcNow, isNew: true);

                await _unitOfWork.PropertyLayout.UpdateAsync(layout);
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

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid layoutId, Guid? agencyId)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                var layout = (await _unitOfWork.PropertyLayout.GetAsync(
                    l => l.Id == layoutId && l.AgencyId == tenantAgencyId)).FirstOrDefault();

                if (layout == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                await _unitOfWork.PropertyLayout.DeleteAsync(layout.Id, Guid.NewGuid());
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


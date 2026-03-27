using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.Repositories;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Linq.Expressions;

namespace PropertyInspection.Application.Services
{
    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Billing> _billingRepository;
        private readonly ITenantContext _tenantContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BillingService> _logger;

        public BillingService(
            IUnitOfWork unitOfWork,
            IGenericRepository<Billing> billingRepository,
            ITenantContext tenantContext,
            IMapper mapper,
            ILogger<BillingService> logger)
        {
            _unitOfWork = unitOfWork;
            _billingRepository = billingRepository;
            _tenantContext = tenantContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<BillingDto>> CreateBillingAsync(CreateBillingDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                {
                    return new ServiceResponse<BillingDto>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var createdBy = ResolveUserId();
                var billing = new Billing
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim(),
                    PriceMonthly = dto.PriceMonthly,
                    PriceYearly = dto.PriceYearly,
                    UserLimits = dto.UserLimits,
                    TrialDays = dto.TrialDays,
                    PropertiesLimit = dto.PropertiesLimit,
                    InspectionsLimit = dto.InspectionsLimit,
                    IsActive = ResolveIsActive(dto.Status),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    Features = MapFeatures(dto.Features)
                };

                await _billingRepository.AddAsync(billing);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<BillingDto>
                {
                    Success = true,
                    Message = "Billing plan created successfully",
                    Data = _mapper.Map<BillingDto>(billing)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating billing plan");
                return new ServiceResponse<BillingDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<BillingDto>> UpdateBillingAsync(Guid id, UpdateBillingDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return new ServiceResponse<BillingDto>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var billing = await _billingRepository.GetByIdAsync(
                    id,
                    include: q => q.Include(b => b.Features));

                if (billing == null || billing.IsDeleted)
                {
                    return new ServiceResponse<BillingDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    billing.Name = dto.Name.Trim();
                }

                if (dto.Description != null)
                {
                    billing.Description = dto.Description.Trim();
                }

                if (dto.PriceMonthly.HasValue)
                {
                    billing.PriceMonthly = dto.PriceMonthly.Value;
                }

                if (dto.PriceYearly.HasValue)
                {
                    billing.PriceYearly = dto.PriceYearly.Value;
                }

                if (dto.UserLimits.HasValue)
                {
                    billing.UserLimits = dto.UserLimits.Value;
                }

                if (dto.TrialDays.HasValue)
                {
                    billing.TrialDays = dto.TrialDays.Value;
                }

                billing.PropertiesLimit = dto.PropertiesLimit;
                billing.InspectionsLimit = dto.InspectionsLimit;

                if (!string.IsNullOrWhiteSpace(dto.Status))
                {
                    billing.IsActive = ResolveIsActive(dto.Status);
                }

                if (dto.Features != null)
                {
                    billing.Features.Clear();
                    foreach (var feature in MapFeatures(dto.Features))
                    {
                        billing.Features.Add(feature);
                    }
                }

                billing.UpdatedAt = DateTime.UtcNow;
                billing.UpdatedBy = ResolveUserId();

                await _billingRepository.UpdateAsync(billing);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<BillingDto>
                {
                    Success = true,
                    Message = "Billing plan updated successfully",
                    Data = _mapper.Map<BillingDto>(billing)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating billing plan {BillingId}", id);
                return new ServiceResponse<BillingDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteBillingAsync(Guid id)
        {
            try
            {
                var billing = await _billingRepository.GetByIdAsync(id);
                if (billing == null || billing.IsDeleted)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                await _billingRepository.DeleteAsync(id, ResolveUserId());
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Billing plan deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting billing plan {BillingId}", id);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> ActivateBillingAsync(Guid id)
        {
            return await SetBillingActiveStateAsync(id, true, "Billing plan activated successfully");
        }

        public async Task<ServiceResponse<bool>> DeactivateBillingAsync(Guid id)
        {
            return await SetBillingActiveStateAsync(id, false, "Billing plan deactivated successfully");
        }

                public async Task<ServiceResponse<IReadOnlyList<BillingDto>>> GetActiveBillingPlansAsync()
        {
            try
            {
                var plans = await _billingRepository.GetAsync(
                    predicate: b => !b.IsDeleted && b.IsActive,
                    include: q => q.Include(b => b.Features),
                    orderBy: q => q.OrderBy(b => b.Name));

                var dtos = _mapper.Map<List<BillingDto>>(plans);

                return new ServiceResponse<IReadOnlyList<BillingDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active billing plans");
                return new ServiceResponse<IReadOnlyList<BillingDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }
public async Task<ServiceResponse<BillingDto>> GetBillingByIdAsync(Guid id)
        {
            try
            {
                var billing = await _billingRepository.GetByIdAsync(
                    id,
                    include: q => q.Include(b => b.Features));

                if (billing == null || billing.IsDeleted)
                {
                    return new ServiceResponse<BillingDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<BillingDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<BillingDto>(billing)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving billing plan {BillingId}", id);
                return new ServiceResponse<BillingDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<PagedResult<BillingDto>>> GetBillingsAsync(BillingFilterDto filter)
        {
            try
            {
                var normalizedFilter = filter ?? new BillingFilterDto();
                var page = normalizedFilter.Page < 1 ? 1 : normalizedFilter.Page;
                var pageSize = normalizedFilter.PageSize < 1 ? 10 : normalizedFilter.PageSize;

                bool? statusFilter = ResolveStatusFilter(normalizedFilter.Status);

                Expression<Func<Billing, bool>> predicate = b =>
                    !b.IsDeleted &&
                    (string.IsNullOrWhiteSpace(normalizedFilter.Search) || b.Name.Contains(normalizedFilter.Search)) &&
                    (!statusFilter.HasValue || b.IsActive == statusFilter.Value) &&
                    (!normalizedFilter.MinPrice.HasValue || b.PriceMonthly >= normalizedFilter.MinPrice.Value) &&
                    (!normalizedFilter.MaxPrice.HasValue || b.PriceMonthly <= normalizedFilter.MaxPrice.Value) &&
                    (!normalizedFilter.FromDate.HasValue || b.CreatedAt >= normalizedFilter.FromDate.Value) &&
                    (!normalizedFilter.ToDate.HasValue || b.CreatedAt <= normalizedFilter.ToDate.Value);

                var (items, totalCount) = await _billingRepository.GetPagedAsync(
                    pageNumber: page,
                    pageSize: pageSize,
                    predicate: predicate,
                    include: q => q.Include(b => b.Features),
                    orderBy: q => q.OrderByDescending(b => b.CreatedAt));

                var dtos = _mapper.Map<List<BillingDto>>(items);
                var result = new PagedResult<BillingDto>
                {
                    Data = dtos,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return new ServiceResponse<PagedResult<BillingDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving billing plans");
                return new ServiceResponse<PagedResult<BillingDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        private async Task<ServiceResponse<bool>> SetBillingActiveStateAsync(Guid id, bool isActive, string successMessage)
        {
            try
            {
                var billing = await _billingRepository.GetByIdAsync(id);
                if (billing == null || billing.IsDeleted)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                billing.IsActive = isActive;
                billing.UpdatedAt = DateTime.UtcNow;
                billing.UpdatedBy = ResolveUserId();

                await _billingRepository.UpdateAsync(billing);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = successMessage,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating billing status {BillingId}", id);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        private static bool ResolveIsActive(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return true;
            }

            var normalized = status.Trim().ToLowerInvariant();
            return normalized == "active" || normalized == "true" || normalized == "1";
        }

        private static bool? ResolveStatusFilter(string? status)
        {
            if (string.IsNullOrWhiteSpace(status) || status.Trim().Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return ResolveIsActive(status);
        }

        private static List<BillingFeature> MapFeatures(List<BillingFeatureDto> features)
        {
            var items = new List<BillingFeature>();
            if (features == null)
            {
                return items;
            }

            foreach (var feature in features)
            {
                if (string.IsNullOrWhiteSpace(feature.Name))
                {
                    continue;
                }

                items.Add(new BillingFeature
                {
                    Id = feature.Id == Guid.Empty ? Guid.NewGuid() : feature.Id,
                    Name = feature.Name.Trim()
                });
            }

            return items;
        }

        private Guid ResolveUserId()
        {
            return Guid.TryParse(_tenantContext.DomainUserId, out var userId) ? userId : Guid.Empty;
        }
    }
}


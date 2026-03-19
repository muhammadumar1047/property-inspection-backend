using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.Services
{
    public class MobileDashboardService : IMobileDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly ITenantContext _tenantContext;

        public MobileDashboardService(
            IUnitOfWork unitOfWork,
            ITenantAgencyResolver tenantAgencyResolver,
            ITenantContext tenantContext)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;
            _tenantContext = tenantContext;
        }

        public async Task<ServiceResponse<DashboardSummaryDto>> GetDashboardAsync(
            Guid? agencyId,
            Guid? inspectorId,
            DateTime? startDate,
            DateTime? endDate)
        {
            try
            {
                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
                var effectiveInspectorId = ResolveInspectorId(inspectorId);

                var baseQuery = _unitOfWork.Inspections
                    .GetQueryable(i =>
                        i.AgencyId == tenantAgencyId &&
                        (!effectiveInspectorId.HasValue || i.InspectorId == effectiveInspectorId.Value))
                    .AsNoTracking();

                var nowUtc = DateTime.UtcNow;
                var todayStart = DateTime.SpecifyKind(nowUtc.Date, DateTimeKind.Utc);
                var todayEnd = todayStart.AddDays(1).AddTicks(-1);

                var weekStart = StartOfWeek(todayStart);
                var weekEnd = weekStart.AddDays(7).AddTicks(-1);

                var monthStart = new DateTime(todayStart.Year, todayStart.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

                var customStart = startDate?.Date;
                var customEnd = endDate.HasValue
                        ? endDate.Value.Date.AddDays(1).AddTicks(-1)
                        : (DateTime?)null;

                if (customStart.HasValue && customEnd.HasValue && customStart.Value > customEnd.Value)
                {
                    return new ServiceResponse<DashboardSummaryDto>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var overallCounts = await GetStatusCountsAsync(baseQuery, nowUtc);
                var todayCounts = await GetStatusCountsAsync(baseQuery.Where(i => i.InspectionDate >= todayStart && i.InspectionDate <= todayEnd), nowUtc);
                var weekCounts = await GetStatusCountsAsync(baseQuery.Where(i => i.InspectionDate >= weekStart && i.InspectionDate <= weekEnd), nowUtc);
                var monthCounts = await GetStatusCountsAsync(baseQuery.Where(i => i.InspectionDate >= monthStart && i.InspectionDate <= monthEnd), nowUtc);

                DashboardPeriodSummaryDto customRange = new()
                {
                    Start = customStart,
                    End = customEnd,
                    StatusCounts = new DashboardStatusCountsDto()
                };

                if (customStart.HasValue && customEnd.HasValue)
                {
                    customRange.StatusCounts = await GetStatusCountsAsync(
                        baseQuery.Where(i => i.InspectionDate >= customStart.Value && i.InspectionDate <= customEnd.Value),
                        nowUtc);
                }

                var recentActivities = await baseQuery
                    .Include(i => i.Property)
                    .Include(i => i.Inspector)
                    .OrderByDescending(i => i.UpdatedAt ?? i.CreatedAt)
                    .Take(10)
                    .Select(i => new DashboardRecentActivityDto
                    {
                        InspectionId = i.Id,
                        PropertyAddress = i.Property == null
                            ? string.Empty
                            : string.Join(" ", new[] { i.Property.Address1, i.Property.Address2 }
                                .Where(x => !string.IsNullOrWhiteSpace(x))),
                        InspectorName = i.Inspector == null
                            ? string.Empty
                            : string.Join(" ", new[] { i.Inspector.FirstName, i.Inspector.LastName }
                                .Where(x => !string.IsNullOrWhiteSpace(x))),
                        Status = i.InspectionStatus,
                        InspectionType = i.InspectionType,
                        InspectionDate = i.InspectionDate,
                        ActivityDate = i.UpdatedAt ?? i.CreatedAt
                    })
                    .ToListAsync();

                var summary = new DashboardSummaryDto
                {
                    TotalAssignedInspections = overallCounts.Total,
                    OverallStatusCounts = overallCounts,
                    Today = new DashboardPeriodSummaryDto
                    {
                        Start = todayStart,
                        End = todayEnd,
                        StatusCounts = todayCounts
                    },
                    ThisWeek = new DashboardPeriodSummaryDto
                    {
                        Start = weekStart,
                        End = weekEnd,
                        StatusCounts = weekCounts
                    },
                    ThisMonth = new DashboardPeriodSummaryDto
                    {
                        Start = monthStart,
                        End = monthEnd,
                        StatusCounts = monthCounts
                    },
                    CustomRange = customRange,
                    RecentActivities = recentActivities
                };

                return new ServiceResponse<DashboardSummaryDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = summary
                };
            }
            catch
            {
                return new ServiceResponse<DashboardSummaryDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        private Guid? ResolveInspectorId(Guid? requestedInspectorId)
        {
            if (_tenantContext.IsSuperAdmin || _tenantContext.IsAgencyAdmin)
            {
                return requestedInspectorId;
            }

            if (Guid.TryParse(_tenantContext.DomainUserId, out var domainUserId))
            {
                return domainUserId;
            }

            return requestedInspectorId;
        }

        private static DateTime StartOfWeek(DateTime date)
        {
            var diff = (7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            return date.AddDays(-diff);
        }

        private static async Task<DashboardStatusCountsDto> GetStatusCountsAsync(IQueryable<Inspection> query, DateTime nowUtc)
        {
            var total = await query.CountAsync();
            var pending = await query.CountAsync(i => i.InspectionStatus == InspectionStatus.Pending);
            var completed = await query.CountAsync(i => i.InspectionStatus == InspectionStatus.Completed || i.InspectionStatus == InspectionStatus.Closed);
            var scheduled = await query.CountAsync(i =>
                i.InspectionStatus == InspectionStatus.Pending &&
                i.InspectionDate >= nowUtc.Date);

            return new DashboardStatusCountsDto
            {
                Total = total,
                Pending = pending,
                Completed = completed,
                Scheduled = scheduled
            };
        }
    }
}


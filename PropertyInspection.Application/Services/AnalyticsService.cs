using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using AutoMapper;

namespace PropertyInspection.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;

        public AnalyticsService(IUnitOfWork unitOfWork, IMapper mapper, ITenantAgencyResolver tenantAgencyResolver)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tenantAgencyResolver = tenantAgencyResolver;
        }

        public async Task<ServiceResponse<AnalyticsSummaryDto>> GetAnalyticsSummaryAsync(AnalyticsFilterDto filter)
        {
            try
            {
                var normalizedFilter = filter ?? new AnalyticsFilterDto();
                var agencyId = _tenantAgencyResolver.ResolveAgencyId(normalizedFilter.AgencyId);
                var (start, end) = NormalizeRange(normalizedFilter);
                var (prevStart, prevEnd) = GetPreviousRange(start, end);

                var totalPropertiesCurrent = await _unitOfWork.Properties.CountAsync(p =>
                    !p.IsDeleted &&
                    p.AgencyId == agencyId &&
                    p.CreatedAt <= end);

                var totalPropertiesPrev = await _unitOfWork.Properties.CountAsync(p =>
                    !p.IsDeleted &&
                    p.AgencyId == agencyId &&
                    p.CreatedAt <= prevEnd);

                var completedCurrent = await CountInspectionsByStatusAsync(agencyId, InspectionStatus.Completed, start, end, normalizedFilter.Status);
                var completedPrev = await CountInspectionsByStatusAsync(agencyId, InspectionStatus.Completed, prevStart, prevEnd, normalizedFilter.Status);

                var pendingCurrent = await CountInspectionsByStatusAsync(agencyId, InspectionStatus.Pending, start, end, normalizedFilter.Status);
                var pendingPrev = await CountInspectionsByStatusAsync(agencyId, InspectionStatus.Pending, prevStart, prevEnd, normalizedFilter.Status);

                var reportsCurrent = await _unitOfWork.Reports.CountAsync(r =>
                    !r.IsDeleted &&
                    r.AgencyId == agencyId &&
                    r.CreatedAt >= start &&
                    r.CreatedAt <= end);

                var reportsPrev = await _unitOfWork.Reports.CountAsync(r =>
                    !r.IsDeleted &&
                    r.AgencyId == agencyId &&
                    r.CreatedAt >= prevStart &&
                    r.CreatedAt <= prevEnd);

                var recentInspectionEntities = await _unitOfWork.Inspections.GetAsync(
                    predicate: i =>
                        !i.IsDeleted &&
                        i.AgencyId == agencyId &&
                        i.InspectionDate >= start &&
                        i.InspectionDate <= end &&
                        (!normalizedFilter.Status.HasValue || (int)i.InspectionStatus == normalizedFilter.Status.Value),
                    include: q => q.Include(i => i.Property).Include(i => i.Inspector),
                    orderBy: q => q.OrderByDescending(i => i.InspectionDate),
                    take: 5);

                var recentInspections = _mapper.Map<List<RecentInspectionDto>>(recentInspectionEntities);

                var now = DateTime.UtcNow;
                var upcomingEnd = now.Date.AddDays(1);
                var upcomingInspectionEntities = await _unitOfWork.Inspections.GetAsync(
                    predicate: i =>
                        !i.IsDeleted &&
                        i.AgencyId == agencyId &&
                        i.InspectionDate >= now.Date &&
                        i.InspectionDate <= upcomingEnd &&
                        (!normalizedFilter.Status.HasValue || (int)i.InspectionStatus == normalizedFilter.Status.Value),
                    include: q => q.Include(i => i.Property).Include(i => i.Inspector),
                    orderBy: q => q.OrderBy(i => i.InspectionDate),
                    take: 5);

                var upcomingInspections = _mapper.Map<List<UpcomingInspectionDto>>(upcomingInspectionEntities);

                var analytics = new AnalyticsSummaryDto
                {
                    TotalProperties = totalPropertiesCurrent,
                    TotalPropertiesChangePercent = CalculateChange(totalPropertiesPrev, totalPropertiesCurrent),
                    CompletedInspections = completedCurrent,
                    CompletedInspectionsChangePercent = CalculateChange(completedPrev, completedCurrent),
                    PendingInspections = pendingCurrent,
                    PendingInspectionsChangePercent = CalculateChange(pendingPrev, pendingCurrent),
                    ReportsGenerated = reportsCurrent,
                    ReportsGeneratedChangePercent = CalculateChange(reportsPrev, reportsCurrent),
                    RecentInspections = recentInspections,
                    UpcomingInspections = upcomingInspections
                };

                return new ServiceResponse<AnalyticsSummaryDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = analytics
                };
            }
            catch
            {
                return new ServiceResponse<AnalyticsSummaryDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<AnalyticsChartDto>> GetAnalyticsChartsAsync(AnalyticsFilterDto filter)
        {
            try
            {
                var normalizedFilter = filter ?? new AnalyticsFilterDto();
                var agencyId = _tenantAgencyResolver.ResolveAgencyId(normalizedFilter.AgencyId);
                var (start, end) = NormalizeRange(normalizedFilter);

                var monthStarts = GetMonthStarts(start, end);
                var labels = monthStarts
                    .Select(m => new DateTime(m.Year, m.Month, 1, 0, 0, 0, DateTimeKind.Utc).ToString("yyyy-MM-01"))
                    .ToList();

                var inspectionsQuery = _unitOfWork.Inspections.GetQueryable(i =>
                    !i.IsDeleted &&
                    i.AgencyId == agencyId &&
                    i.InspectionDate >= start &&
                    i.InspectionDate <= end);

                if (normalizedFilter.Status.HasValue)
                {
                    inspectionsQuery = inspectionsQuery.Where(i => (int)i.InspectionStatus == normalizedFilter.Status.Value);
                }

                var grouped = await inspectionsQuery
                    .GroupBy(i => new { i.InspectionType, i.InspectionDate.Year, i.InspectionDate.Month })
                    .Select(g => new
                    {
                        Type = g.Key.InspectionType,
                        g.Key.Year,
                        g.Key.Month,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var datasets = new List<AnalyticsChartDatasetDto>();
                foreach (var type in Enum.GetValues<InspectionType>())
                {
                    var data = new List<int>();
                    foreach (var month in monthStarts)
                    {
                        var hit = grouped.FirstOrDefault(g =>
                            g.Type == type &&
                            g.Year == month.Year &&
                            g.Month == month.Month);
                        data.Add(hit?.Count ?? 0);
                    }

                    datasets.Add(new AnalyticsChartDatasetDto
                    {
                        Label = type.ToString(),
                        Data = data
                    });
                }

                return new ServiceResponse<AnalyticsChartDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = new AnalyticsChartDto
                    {
                        Labels = labels,
                        Datasets = datasets
                    }
                };
            }
            catch
            {
                return new ServiceResponse<AnalyticsChartDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<AnalyticsDto>> GetDashboardAnalyticsByAgencyAsync(Guid? agencyId)
        {
            try
            {
                var summaryResponse = await GetAnalyticsSummaryAsync(new AnalyticsFilterDto { AgencyId = agencyId });
                if (!summaryResponse.Success || summaryResponse.Data == null)
                {
                    return new ServiceResponse<AnalyticsDto>
                    {
                        Success = false,
                        Message = summaryResponse.Message,
                        ErrorCode = summaryResponse.ErrorCode ?? ServiceErrorCodes.ServerError
                    };
                }

                var chartsResponse = await GetAnalyticsChartsAsync(new AnalyticsFilterDto { AgencyId = agencyId });
                if (!chartsResponse.Success || chartsResponse.Data == null)
                {
                    return new ServiceResponse<AnalyticsDto>
                    {
                        Success = false,
                        Message = chartsResponse.Message,
                        ErrorCode = chartsResponse.ErrorCode ?? ServiceErrorCodes.ServerError
                    };
                }

                var analytics = new AnalyticsDto
                {
                    TotalProperties = summaryResponse.Data.TotalProperties,
                    TotalPropertiesChangePercent = summaryResponse.Data.TotalPropertiesChangePercent,
                    CompletedInspections = summaryResponse.Data.CompletedInspections,
                    CompletedInspectionsChangePercent = summaryResponse.Data.CompletedInspectionsChangePercent,
                    PendingInspections = summaryResponse.Data.PendingInspections,
                    PendingInspectionsChangePercent = summaryResponse.Data.PendingInspectionsChangePercent,
                    ReportsGenerated = summaryResponse.Data.ReportsGenerated,
                    ReportsGeneratedChangePercent = summaryResponse.Data.ReportsGeneratedChangePercent,
                    RecentInspections = summaryResponse.Data.RecentInspections,
                    UpcomingInspections = summaryResponse.Data.UpcomingInspections,
                    MonthlyInspections = new List<MonthlyInspectionDto>(),
                    InspectionsByType = new List<InspectionTypeDistributionDto>(),
                    TopSuburbs = new List<TopSuburbDto>()
                };

                return new ServiceResponse<AnalyticsDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = analytics
                };
            }
            catch
            {
                return new ServiceResponse<AnalyticsDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        private static (DateTime Start, DateTime End) NormalizeRange(AnalyticsFilterDto filter)
        {
            var now = DateTime.UtcNow.Date;
            var start = filter.StartDate?.Date ?? new DateTime(now.Year, now.Month, 1).AddMonths(-11);
            var end = filter.EndDate?.Date ?? now;

            if (end < start)
            {
                (start, end) = (end, start);
            }

            return (DateTime.SpecifyKind(start, DateTimeKind.Utc), DateTime.SpecifyKind(end, DateTimeKind.Utc));
        }

        private static (DateTime Start, DateTime End) GetPreviousRange(DateTime start, DateTime end)
        {
            var days = (end - start).Days + 1;
            var prevEnd = start.AddDays(-1);
            var prevStart = prevEnd.AddDays(-days + 1);
            return (prevStart, prevEnd);
        }

        private static List<DateTime> GetMonthStarts(DateTime start, DateTime end)
        {
            var result = new List<DateTime>();
            var cursor = new DateTime(start.Year, start.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var last = new DateTime(end.Year, end.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            while (cursor <= last)
            {
                result.Add(cursor);
                cursor = cursor.AddMonths(1);
            }

            return result;
        }

        private static double CalculateChange(int last, int current)
        {
            return last == 0 ? (current > 0 ? 100 : 0) : Math.Round(((double)(current - last) / last) * 100, 2);
        }

        private async Task<int> CountInspectionsByStatusAsync(Guid agencyId, InspectionStatus status, DateTime start, DateTime end, int? statusFilter)
        {
            if (statusFilter.HasValue && statusFilter.Value != (int)status)
            {
                return 0;
            }

            return await _unitOfWork.Inspections.CountAsync(i =>
                !i.IsDeleted &&
                i.AgencyId == agencyId &&
                i.InspectionStatus == status &&
                i.InspectionDate >= start &&
                i.InspectionDate <= end);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;
using AutoMapper;

namespace PropertyInspection.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnalyticsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AnalyticsDto>> GetDashboardAnalyticsByAgencyAsync(Guid agencyId)
        {
            try
            {
                var now = DateTime.UtcNow;
                var startOfThisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var startOfLastMonth = startOfThisMonth.AddMonths(-1);
                var endOfLastMonth = startOfThisMonth.AddTicks(-1);

                var totalProperties = await _unitOfWork.Properties.CountAsync(p => p.AgencyId == agencyId);
                var completedThisMonth = await _unitOfWork.Inspections.CountAsync(i =>
                    i.AgencyId == agencyId &&
                    i.InspectionStatus == InspectionStatus.Completed &&
                    i.InspectionDate >= startOfThisMonth);
                var completedLastMonth = await _unitOfWork.Inspections.CountAsync(i =>
                    i.AgencyId == agencyId &&
                    i.InspectionStatus == InspectionStatus.Completed &&
                    i.InspectionDate >= startOfLastMonth &&
                    i.InspectionDate <= endOfLastMonth);

                var pendingThisMonth = await _unitOfWork.Inspections.CountAsync(i =>
                    i.AgencyId == agencyId &&
                    i.InspectionStatus == InspectionStatus.Pending &&
                    i.InspectionDate >= startOfThisMonth);
                var pendingLastMonth = await _unitOfWork.Inspections.CountAsync(i =>
                    i.AgencyId == agencyId &&
                    i.InspectionStatus == InspectionStatus.Pending &&
                    i.InspectionDate >= startOfLastMonth &&
                    i.InspectionDate <= endOfLastMonth);

                var reportsThisMonth = await _unitOfWork.Reports.CountAsync(r =>
                    r.AgencyId == agencyId &&
                    r.CreatedAt >= startOfThisMonth);
                var reportsLastMonth = await _unitOfWork.Reports.CountAsync(r =>
                    r.AgencyId == agencyId &&
                    r.CreatedAt >= startOfLastMonth &&
                    r.CreatedAt <= endOfLastMonth);

                double CalculateChange(int last, int current) =>
                    last == 0 ? (current > 0 ? 100 : 0) : Math.Round(((double)(current - last) / last) * 100, 2);

                var recentInspectionEntities = await _unitOfWork.Inspections.GetAsync(
                    predicate: i => i.AgencyId == agencyId,
                    include: q => q.Include(i => i.Property).Include(i => i.Inspector),
                    orderBy: q => q.OrderByDescending(i => i.InspectionDate),
                    take: 5);
                var recentInspections = _mapper.Map<List<RecentInspectionDto>>(recentInspectionEntities);

                var upcomingInspectionEntities = await _unitOfWork.Inspections.GetAsync(
                    predicate: i =>
                        i.AgencyId == agencyId &&
                        i.InspectionDate >= now.Date,
                    include: q => q.Include(i => i.Property).Include(i => i.Inspector),
                    orderBy: q => q.OrderBy(i => i.InspectionDate),
                    take: 5);
                var upcomingInspections = _mapper.Map<List<UpcomingInspectionDto>>(upcomingInspectionEntities);

                var monthlyInspectionsRaw = await _unitOfWork.Inspections.GetAsync(
                    predicate: i =>
                        i.AgencyId == agencyId &&
                        i.InspectionDate >= startOfThisMonth.AddMonths(-11),
                    orderBy: q => q.OrderBy(i => i.InspectionDate));

                var monthlyInspections = monthlyInspectionsRaw
                    .GroupBy(i => new { i.InspectionDate.Year, i.InspectionDate.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new MonthlyInspectionDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Total = g.Count()
                    }).ToList();

                var byType = (await _unitOfWork.Inspections.GetAsync(
                    predicate: i => i.AgencyId == agencyId))
                    .GroupBy(i => i.InspectionType)
                    .Select(g => new InspectionTypeDistributionDto
                    {
                        Type = g.Key.ToString(),
                        Count = g.Count()
                    }).ToList();

                var topSuburbs = (await _unitOfWork.Properties.GetAsync(
                    predicate: p => p.AgencyId == agencyId))
                    .GroupBy(p => p.CityOrSuburb)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new TopSuburbDto
                    {
                        SuburbName = g.Key,
                        Count = g.Count()
                    }).ToList();

                var analytics = new AnalyticsDto
                {
                    TotalProperties = totalProperties,
                    CompletedInspections = completedThisMonth,
                    CompletedInspectionsChangePercent = CalculateChange(completedLastMonth, completedThisMonth),
                    PendingInspections = pendingThisMonth,
                    PendingInspectionsChangePercent = CalculateChange(pendingLastMonth, pendingThisMonth),
                    ReportsGenerated = reportsThisMonth,
                    ReportsGeneratedChangePercent = CalculateChange(reportsLastMonth, reportsThisMonth),
                    RecentInspections = recentInspections,
                    UpcomingInspections = upcomingInspections,
                    MonthlyInspections = monthlyInspections,
                    InspectionsByType = byType,
                    TopSuburbs = topSuburbs
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
    }
}

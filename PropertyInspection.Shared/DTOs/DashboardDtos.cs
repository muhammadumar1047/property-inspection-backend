using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalAssignedInspections { get; set; }
        public DashboardStatusCountsDto OverallStatusCounts { get; set; } = new();
        public DashboardPeriodSummaryDto Today { get; set; } = new();
        public DashboardPeriodSummaryDto ThisWeek { get; set; } = new();
        public DashboardPeriodSummaryDto ThisMonth { get; set; } = new();
        public DashboardPeriodSummaryDto CustomRange { get; set; } = new();
        public List<DashboardRecentActivityDto> RecentActivities { get; set; } = new();
    }

    public class DashboardStatusCountsDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Completed { get; set; }
        public int Scheduled { get; set; }
    }

    public class DashboardPeriodSummaryDto
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DashboardStatusCountsDto StatusCounts { get; set; } = new();
    }

    public class DashboardRecentActivityDto
    {
        public Guid InspectionId { get; set; }
        public string PropertyAddress { get; set; } = string.Empty;
        public string InspectorName { get; set; } = string.Empty;
        public InspectionStatus Status { get; set; }
        public InspectionType InspectionType { get; set; }
        public DateTime InspectionDate { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}

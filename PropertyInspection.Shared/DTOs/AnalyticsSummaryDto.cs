using System.Collections.Generic;

namespace PropertyInspection.Shared.DTOs
{
    public class AnalyticsSummaryDto
    {
        public int TotalProperties { get; set; }
        public double TotalPropertiesChangePercent { get; set; }
        public int CompletedInspections { get; set; }
        public double CompletedInspectionsChangePercent { get; set; }
        public int PendingInspections { get; set; }
        public double PendingInspectionsChangePercent { get; set; }
        public int ReportsGenerated { get; set; }
        public double ReportsGeneratedChangePercent { get; set; }
        public List<RecentInspectionDto> RecentInspections { get; set; } = new();
        public List<UpcomingInspectionDto> UpcomingInspections { get; set; } = new();
    }
}

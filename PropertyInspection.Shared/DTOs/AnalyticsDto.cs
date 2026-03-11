using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class AnalyticsDto
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
        public List<MonthlyInspectionDto> MonthlyInspections { get; set; } = new();
        public List<InspectionTypeDistributionDto> InspectionsByType { get; set; } = new();
        public List<TopSuburbDto> TopSuburbs { get; set; } = new();
    }
}

using System;

namespace PropertyInspection.Shared.DTOs
{
    public class AnalyticsFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? AgencyId { get; set; }
        public int? Status { get; set; }
    }
}

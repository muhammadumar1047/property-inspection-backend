using System;
using System.Collections.Generic;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportDto : BaseEntityDto
    {
        public Guid InspectionId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public InspectionResponse? Inspection { get; set; }
        public ReportHeaderDto Header { get; set; } = new();
        public List<ReportAreaDto> Areas { get; set; } = new();
    }
}

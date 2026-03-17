using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportDto : BaseEntityDto
    {
        public Guid InspectionId { get; set; }
        public string ReportType { get; set; } = null!;
        public string? Notes { get; set; }

        [JsonIgnore]
        public InspectionResponse? Inspection { get; set; }

        public List<ReportAreaDto> ReportAreas { get; set; } = new();
    }
}


using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportAreaDto
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public string Name { get; set; } = null!;

        [JsonIgnore]
        public ReportDto? Report { get; set; }

        public List<ReportItemDto> ReportItems { get; set; } = new();
    }
}

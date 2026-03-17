using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportItemConditionDto
    {
        public Guid Id { get; set; }
        public Guid ReportItemId { get; set; }
        public string Description { get; set; } = null!;
        public string? Value { get; set; }

        [JsonIgnore]
        public ReportItemDto? ReportItem { get; set; }
    }
}

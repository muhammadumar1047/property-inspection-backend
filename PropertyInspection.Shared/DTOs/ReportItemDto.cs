using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportItemDto
    {
        public Guid Id { get; set; }
        public Guid ReportAreaId { get; set; }
        public string Name { get; set; } = null!;

        [JsonIgnore]
        public ReportAreaDto? ReportArea { get; set; }

        public List<ReportItemConditionDto> ReportItemConditions { get; set; } = new();
        public List<ReportItemCommentDto> ReportItemComments { get; set; } = new();
        public List<ReportMediaDto> ReportMedia { get; set; } = new();
    }
}

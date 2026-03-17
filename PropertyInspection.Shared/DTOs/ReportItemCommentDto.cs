using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportItemCommentDto
    {
        public Guid Id { get; set; }
        public Guid ReportItemId { get; set; }
        public string Text { get; set; } = null!;

        [JsonIgnore]
        public ReportItemDto? ReportItem { get; set; }
    }
}

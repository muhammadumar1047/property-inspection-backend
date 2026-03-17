using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportMediaDto
    {
        public Guid Id { get; set; }
        public Guid ReportItemId { get; set; }
        public string Url { get; set; } = null!;
        public MediaType Type { get; set; }

        [JsonIgnore]
        public ReportItemDto? ReportItem { get; set; }

        public List<ReportMediaCommentDto> ReportMediaComments { get; set; } = new();
    }
}

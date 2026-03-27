using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportMediaCommentDto
    {
        public Guid Id { get; set; }
        public Guid ReportMediaId { get; set; }
        public string Text { get; set; } = null!;
        public decimal? X { get; set; }
        public decimal? Y { get; set; }

        [JsonIgnore]
        public ReportMediaDto? ReportMedia { get; set; }
    }
}

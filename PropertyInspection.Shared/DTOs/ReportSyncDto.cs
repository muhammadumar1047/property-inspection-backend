using System;
using System.Collections.Generic;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportSyncDto
    {

        public Guid? AgencyId { get; set; }
        public Guid ReportId { get; set; }
        public Guid InspectionId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ReportSyncAreaDto> ReportAreas { get; set; } = new();
    }

    public class ReportSyncAreaDto
    {
        public Guid ReportAreaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ReportSyncItemDto> ReportItems { get; set; } = new();
    }

    public class ReportSyncItemDto
    {
        public Guid ReportItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ReportSyncConditionDto> ReportItemConditions { get; set; } = new();
        public List<ReportSyncCommentDto> ReportItemComments { get; set; } = new();
        public List<ReportSyncMediaDto> ReportMedia { get; set; } = new();
    }

    public class ReportSyncConditionDto
    {
        public Guid ReportItemConditionId { get; set; }
        public Guid ReportItemId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public object? Value { get; set; }
    }

    public class ReportSyncCommentDto
    {
        public Guid ReportItemCommentId { get; set; }
        public Guid ReportItemId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class ReportSyncMediaDto
    {
        public Guid ReportMediaId { get; set; }
        public Guid ReportItemId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<ReportSyncMediaCommentDto> ReportMediaComments { get; set; } = new();
    }

    public class ReportSyncMediaCommentDto
    {
        public Guid ReportMediaCommentId { get; set; }
        public Guid ReportMediaId { get; set; }
        public string Text { get; set; } = string.Empty;
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
    }
}

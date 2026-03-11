using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportTemplateDto
    {
        public Guid ReportId { get; set; }
        public Guid InspectionId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ReportTemplateAreaDto> ReportAreas { get; set; } = new();
    }

    public class ReportTemplateAreaDto
    {
        public Guid ReportAreaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ReportTemplateItemDto> ReportItems { get; set; } = new();
    }

    public class ReportTemplateItemDto
    {
        public Guid ReportItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ReportTemplateItemConditionDto> ReportItemConditions { get; set; } = new();
        public List<ReportTemplateItemCommentDto> ReportItemComments { get; set; } = new();
        public List<ReportTemplateMediaDto> ReportMedia { get; set; } = new();
    }

    public class ReportTemplateItemConditionDto
    {
        public Guid ReportItemConditionId { get; set; }
        public Guid ReportItemId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class ReportTemplateItemCommentDto
    {
        public Guid ReportItemCommentId { get; set; }
        public Guid ReportItemId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class ReportTemplateMediaDto
    {
        public Guid ReportMediaId { get; set; }
        public Guid ReportItemId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<ReportTemplateMediaCommentDto> ReportMediaComments { get; set; } = new();
    }

    public class ReportTemplateMediaCommentDto
    {
        public Guid ReportMediaCommentId { get; set; }
        public Guid ReportMediaId { get; set; }
        public string Text { get; set; } = string.Empty;
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
    }

}

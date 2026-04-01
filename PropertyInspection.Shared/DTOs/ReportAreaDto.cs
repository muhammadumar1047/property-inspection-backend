using System;
using System.Collections.Generic;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportAreaDto
    {
        public Guid AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public List<ReportItemDto> Items { get; set; } = new();
    }
}

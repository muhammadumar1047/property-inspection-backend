using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class LayoutItemDto : BaseEntityDto
    {
        public Guid? AreaId { get; set; }
        public string ItemName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public LayoutAreaDto? Area { get; set; }
    }
}

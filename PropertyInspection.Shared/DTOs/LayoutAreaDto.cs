using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class LayoutAreaDto : BaseEntityDto
    {
        public Guid? LayoutId { get; set; }
        public string AreaName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public PropertyLayoutDto? Layout { get; set; }

        public List<LayoutItemDto> LayoutItem { get; set; } = new List<LayoutItemDto>();
    }
}

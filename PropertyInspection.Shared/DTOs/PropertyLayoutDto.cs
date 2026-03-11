using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class PropertyLayoutDto : BaseEntityDto
    {
        public Guid? AgencyId { get; set; }
        public PropertyType LayoutType { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public AgencyDto? Agency { get; set; }
        public List<LayoutAreaDto> LayoutArea { get; set; } = new List<LayoutAreaDto>();
    }


    public class CreatePropertyLayoutDto
    {
        public Guid? AgencyId { get; set; }
        public PropertyType LayoutType { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public List<CreateLayoutAreaDto> LayoutArea { get; set; } = new List<CreateLayoutAreaDto>();
    }

    public class CreateLayoutAreaDto 
    {
        public string AreaName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public PropertyLayoutDto? Layout { get; set; }

        public List<CreateLayoutItemDto> LayoutItem { get; set; } = new List<CreateLayoutItemDto>();
    }

    public class CreateLayoutItemDto
    {
        public string ItemName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public LayoutAreaDto? Area { get; set; }
    }
}

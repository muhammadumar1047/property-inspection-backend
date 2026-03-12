using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class PropertyLayoutResponse : BaseEntityDto
    {
        public Guid? AgencyId { get; set; }
        public PropertyType LayoutType { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public AgencyResponse? Agency { get; set; }
        public List<LayoutAreaResponse> LayoutArea { get; set; } = new List<LayoutAreaResponse>();
    }


    public class CreatePropertyLayoutRequest
    {
        public Guid? AgencyId { get; set; }
        public PropertyType LayoutType { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public List<CreateLayoutAreaRequest> LayoutArea { get; set; } = new List<CreateLayoutAreaRequest>();
    }

    public class CreateLayoutAreaRequest 
    {
        public string AreaName { get; set; } = null!;
        public int DisplayOrder { get; set; }

        public List<CreateLayoutItemRequest> LayoutItem { get; set; } = new List<CreateLayoutItemRequest>();
    }

    public class CreateLayoutItemRequest
    {
        public string ItemName { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}


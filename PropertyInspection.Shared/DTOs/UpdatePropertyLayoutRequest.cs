using System;
using System.Collections.Generic;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class UpdatePropertyLayoutRequest
    {
        public Guid Id { get; set; }
        public Guid? AgencyId { get; set; }
        public PropertyType LayoutType { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public List<UpdateLayoutAreaRequest> LayoutArea { get; set; } = new List<UpdateLayoutAreaRequest>();
    }

    public class UpdateLayoutAreaRequest
    {
        public Guid Id { get; set; }
        public string AreaName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public List<UpdateLayoutItemRequest> LayoutItem { get; set; } = new List<UpdateLayoutItemRequest>();
    }

    public class UpdateLayoutItemRequest
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}

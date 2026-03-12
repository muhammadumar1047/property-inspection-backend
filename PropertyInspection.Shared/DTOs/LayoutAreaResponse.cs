using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class LayoutAreaResponse : BaseEntityDto
    {
        public Guid? LayoutId { get; set; }
        public string AreaName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        [JsonIgnore]
        public PropertyLayoutResponse? Layout { get; set; }

        public List<LayoutItemResponse> LayoutItem { get; set; } = new List<LayoutItemResponse>();
    }
}


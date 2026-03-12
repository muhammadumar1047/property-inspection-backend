using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class LayoutItemResponse : BaseEntityDto
    {
        public Guid? AreaId { get; set; }
        public string ItemName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        [JsonIgnore]
        public LayoutAreaResponse? Area { get; set; }
    }
}


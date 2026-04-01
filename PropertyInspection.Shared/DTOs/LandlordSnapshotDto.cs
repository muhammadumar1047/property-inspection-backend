using System;
using PropertyInspection.Core.Enums;
using System.Text.Json.Serialization;

namespace PropertyInspection.Shared.DTOs
{
    public class LandlordSnapshotDto : BaseEntityDto
    {
        public Guid InspectionId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [JsonIgnore]
        public InspectionResponse? Inspection { get; set; }
    }
}


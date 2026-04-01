using System;
using PropertyInspection.Core.Enums;
using System.Text.Json.Serialization;

namespace PropertyInspection.Shared.DTOs
{
    public class TenancySnapshotDto : BaseEntityDto
    {
        public Guid InspectionId { get; set; }
        public string TenantName { get; set; } = null!;
        public string TenantEmail { get; set; } = null!;
        public string? TenantPhone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RentAmount { get; set; }
        [JsonIgnore]
        public InspectionResponse? Inspection { get; set; }
    }
}


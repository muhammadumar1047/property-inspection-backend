using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class LandlordSnapshotDto : BaseEntityDto
    {
        public Guid InspectionId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public InspectionResponse? Inspection { get; set; }
    }
}


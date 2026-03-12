using System;
using PropertyInspection.Core.Enums;
using PropertyInspection.Shared.Auth;

namespace PropertyInspection.Shared.DTOs
{
    public class InspectionResponse
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public Guid PropertyId { get; set; }
        public string PropertyAddress { get; set; } = string.Empty;
        public string PropertySubhurb { get; set; } = string.Empty;
        public Guid InspectorId { get; set; }
        public string InspectorName { get; set; }
        public InspectionType InspectionType { get; set; }
        public InspectionStatus InspectionStatus { get; set; }
        public DateTime InspectionDate { get; set; }
        public TimeSpan InspectionTime { get; set; }
        public PropertyResponse? Property { get; set; }
        public AgencyResponse? Agency { get; set; }
        public UserResponse? Inspector { get; set; }

        public bool isActive { get; set; }
        public List<LandlordSnapshotDto>? LandlordSnapshots { get; set; }
        public List<TenancySnapshotDto>? TenancySnapshots { get; set; }
    }
}


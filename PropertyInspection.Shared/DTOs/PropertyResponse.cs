using System;
using PropertyInspection.Core.Enums;
using PropertyInspection.Shared.Auth;

namespace PropertyInspection.Shared.DTOs
{
    public class PropertyResponse : BaseEntityDto
    {
        public Guid? AgencyId { get; set; }
        public string Name { get; set; } = null!;
        public PropertyType Type { get; set; }
        public Guid PropertyManagerId { get; set; }
        public string Address1 { get; set; } = null!;
        public string? Address2 { get; set; }
        public string CityOrSuburb { get; set; } = null!;
        public Guid StateLookupId { get; set; }
        public string Postcode { get; set; } = null!;
        public InspectionFrequencyType InspectionFrequencyType { get; set; }
        public int InspectionFrequencyNumber { get; set; }
        public string? KeyNo { get; set; }
        public string? AlarmCode { get; set; }
        public string? PropertyNotes { get; set; }
        public string? PropertyImages { get; set; }
        public Guid PropertyLayoutId { get; set; }
        public string? PropertyManagerName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public AgencyResponse? Agency { get; set; }
        public UserResponse? PropertyManager { get; set; }
        public StateLookupDto? State { get; set; }
        public PropertyLayoutResponse? PropertyLayout { get; set; }
        public List<LandlordDto> Landlords { get; set; } = new List<LandlordDto>();
        public List<TenancyDto> Tenancies { get; set; } = new List<TenancyDto>();
    }
}


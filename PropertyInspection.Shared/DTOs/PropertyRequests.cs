using System;
using System.Collections.Generic;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class PropertyRequestBase
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
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<LandlordDto> Landlords { get; set; } = new List<LandlordDto>();
        public List<TenancyDto> Tenancies { get; set; } = new List<TenancyDto>();
    }

    public class CreatePropertyRequest : PropertyRequestBase
    {
    }

    public class UpdatePropertyRequest : PropertyRequestBase
    {
        public Guid Id { get; set; }
    }
}

using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class TimeZoneLookupDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = null!;
        public string TimeZoneId { get; set; } = null!;
        public Guid? CountryId { get; set; }
        public CountryLookupDto? Country { get; set; }
    }
}

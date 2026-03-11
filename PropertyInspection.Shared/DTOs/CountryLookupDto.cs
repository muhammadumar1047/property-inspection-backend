using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class CountryLookupDto 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string IsoAlpha3 { get; set; } = null!;
        public string IsoAlpha2 { get; set; } = null!;
    }
}

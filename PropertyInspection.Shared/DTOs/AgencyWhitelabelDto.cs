using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class AgencyWhitelabelDto
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public AgencyDto? Agency { get; set; }
        public string AgencyNameColor { get; set; } = null!;
        public string AddressColor { get; set; } = null!;
        public string? AccentColor { get; set; }
        public string? AccentFontFamily { get; set; }
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? FontFamily { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

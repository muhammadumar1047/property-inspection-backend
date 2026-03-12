using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class AgencyWhitelabelResponse
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        [JsonIgnore]
        public AgencyResponse? Agency { get; set; }
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


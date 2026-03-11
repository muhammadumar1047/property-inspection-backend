using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class CreateAgencyWhitelabelDto
    {
        public Guid? AgencyId { get; set; }
        public string AgencyNameColor { get; set; } = "#1E40AF";
        public string AddressColor { get; set; } = "#1E40AF";
        public string? AccentColor { get; set; } = "#10B981";
        public string? AccentFontFamily { get; set; } = "Arial, sans-serif";
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; } = "#1E40AF";
        public string? SecondaryColor { get; set; } = "#EF4444";
        public string? FontFamily { get; set; } = "Arial, sans-serif";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class WhitelabelBrandingDto
    {
        public string AgencyNameColor { get; set; } = string.Empty;
        public string AddressColor { get; set; } = string.Empty;
        public string? AccentColor { get; set; }
        public string? AccentFontFamily { get; set; }
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? FontFamily { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class AgencyWhitelabel : BaseEntity
    {
        [Required]
        public Guid AgencyId { get; set; }

        public virtual Agency Agency { get; set; } = null!;

        [Required]
        [StringLength(7)]
        public string AgencyNameColor { get; set; } = "#1E40AF"; 

        [Required]
        [StringLength(7)]
        public string AddressColor { get; set; } = "#1E40AF";

        [StringLength(7)]
        public string? AccentColor { get; set; } = "#10B981"; 

        [StringLength(100)]
        public string? AccentFontFamily { get; set; } = "Arial, sans-serif"; 

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [StringLength(7)]
        public string? PrimaryColor { get; set; } = "#1E40AF";

        [StringLength(7)]
        public string? SecondaryColor { get; set; } = "#EF4444";

        [StringLength(100)]
        public string? FontFamily { get; set; } = "Arial, sans-serif";
    }
}

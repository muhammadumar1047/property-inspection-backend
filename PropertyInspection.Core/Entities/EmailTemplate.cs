using PropertyInspection.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyInspection.Core.Entities
{
    public class EmailTemplate : BaseEntity
    {
        [Required]
        public Guid AgencyId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = null!;

        [Required, StringLength(250)]
        public string Subject { get; set; } = null!;

        [Required]
        public InspectionType InspectionType { get; set; }

        public bool IsDefault { get; set; }

        [Required]
        public string Body { get; set; } = null!;

        [StringLength(100)]
        public string? FontFamily { get; set; }

        [StringLength(10)]
        public string? LineSpacing { get; set; }

        [StringLength(20)]
        public string? PrimaryColor { get; set; }

        [StringLength(20)]
        public string? AccentColor { get; set; }

        [StringLength(20)]
        public string? BackgroundColor { get; set; }

        [Required]
        public TemplateStatus Status { get; set; } = TemplateStatus.Published;

        [ForeignKey(nameof(AgencyId))]
        public virtual Agency Agency { get; set; } = null!;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class EmailTemplateResponse
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public string Name { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public InspectionType InspectionType { get; set; }
        public bool IsDefault { get; set; }
        public string Body { get; set; } = null!;
        public string? FontFamily { get; set; }
        public string? LineSpacing { get; set; }
        public string? PrimaryColor { get; set; }
        public string? AccentColor { get; set; }
        public string? BackgroundColor { get; set; }
        public string Status { get; set; } = null!; // "Draft", "Published"
        public string Snippet { get; set; } = null!;
        public DateTime LastUpdated { get; set; }
    }

    public class CreateEmailTemplateRequest
    {
        public Guid? AgencyId { get; set; }

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
        public string Status { get; set; } = "Published"; // "Draft" or "Published"
    }

    public class UpdateEmailTemplateRequest
    {
        public Guid? AgencyId { get; set; }

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
        public string Status { get; set; } = "Published";
    }

    public class MakeDefaultRequest
    {
        public Guid? AgencyId { get; set; }
    }

    public class SendTestEmailRequest
    {
        [Required, EmailAddress]
        public string To { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Body { get; set; } = null!;
    }
}

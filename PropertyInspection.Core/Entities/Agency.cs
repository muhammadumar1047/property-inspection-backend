using System.ComponentModel.DataAnnotations;

namespace PropertyInspection.Core.Entities
{
    public class Agency : BaseEntity
    {

        [Required, StringLength(200)] public string LegalBusinessName { get; set; } = string.Empty;
        [Required, StringLength(500)] public string Address { get; set; } = string.Empty;
        [StringLength(100)] public string? Suburb { get; set; }
        [StringLength(100)] public string? City { get; set; }
        public Guid CountryId { get; set; }
        public CountryLookup? Country { get; set; }
        public Guid StateId { get; set; }
        public StateLookup? State { get; set; }
        [StringLength(10)] public string? Postcode { get; set; }
        [StringLength(20)] public string? PhoneNumber { get; set; }
        [StringLength(20)] public string? FaxNumber { get; set; }
        public Guid TimeZoneId { get; set; }
        public TimeZoneLookup? TimeZone { get; set; }
        [StringLength(255)] public string? CompanyWebsite { get; set; }

        // Main Contact
        [StringLength(100)] public string? ContactPersonFirstName { get; set; }
        [StringLength(100)] public string? ContactPersonLastName { get; set; }
        [StringLength(20)] public string? ContactPersonPhone { get; set; }
        [StringLength(20)] public string? ContactPersonFaxNumber { get; set; }
        [StringLength(20)] public string? ContactPersonJobTitle { get; set; }
        [StringLength(100)] public string? ContactPersonEmail { get; set; }

        // Billing
        [StringLength(100)] public string? BillingContactFirstName { get; set; }
        [StringLength(100)] public string? BillingContactLastName { get; set; }
        [StringLength(20)] public string? BillingPhoneNumber { get; set; }
        [StringLength(200)] public string? BillingContactJobTitle { get; set; }
        [StringLength(200)] public string? BillingFaxNumber { get; set; }
        [StringLength(100)] public string? BillingContactEmail { get; set; }

        // Technical
        [StringLength(100)] public string? TechnicalContactFirstName { get; set; }
        [StringLength(100)] public string? TechnicalContactLastName { get; set; }
        [StringLength(20)] public string? TechnicalPhoneNumber { get; set; }
        [StringLength(200)] public string? TechnicalContactJobTitle { get; set; }
        [StringLength(200)] public string? TechnicalContactFaxNumber { get; set; }
        [StringLength(100)] public string? TechnicalContactEmail { get; set; }

        public AgencyWhitelabel AgencyWhitelabel { get; set; } = null!;
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}

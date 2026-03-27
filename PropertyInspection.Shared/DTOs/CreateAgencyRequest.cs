using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class CreateAgencyRequest
    {
        public string LegalBusinessName { get; set; } = string.Empty;
        public string? CompanyWebsite { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public string? Postcode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public Guid? TimeZoneId { get; set; }
        [Required]
        public string AdminUsername { get; set; } = string.Empty;
        [Required]
        public string AdminFirstName { get; set; } = string.Empty;

        [Required]
        public string AdminLastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string AdminEmail { get; set; } = string.Empty;

        [Required]
        public string AdminPassword { get; set; } = string.Empty;

        public string? ContactPersonFirstName { get; set; }
        public string? ContactPersonLastName { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonJobTitle { get; set; }
        public string? ContactPersonFaxNumber { get; set; }
        public string? ContactPersonEmail { get; set; }

        public string? BillingContactFirstName { get; set; }
        public string? BillingContactLastName { get; set; }
        public string? BillingPhoneNumber { get; set; }
        public string? BillingContactJobTitle { get; set; }
        public string? BillingFaxNumber { get; set; }
        public string? BillingContactEmail { get; set; }

        public string? TechnicalContactFirstName { get; set; }
        public string? TechnicalContactLastName { get; set; }
        public string? TechnicalPhoneNumber { get; set; }
        public string? TechnicalContactJobTitle { get; set; }
        public string? TechnicalContactFaxNumber { get; set; }
        public string? TechnicalContactEmail { get; set; }

        [Required]
        public Guid? BillingPlanId { get; set; }
    }

}


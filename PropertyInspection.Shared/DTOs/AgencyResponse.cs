using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class AgencyResponse : BaseEntityDto
    {    
        public string LegalBusinessName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public Guid? CountryId { get; set; }
        public CountryLookupDto? Country { get; set; }
        public Guid? StateId { get; set; }
        public StateLookupDto? State { get; set; }
        public string? Postcode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public Guid? TimeZoneId { get; set; }
        public TimeZoneLookupDto? TimeZone { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? ContactPersonFirstName { get; set; }
        public string? ContactPersonLastName { get; set; }
        public string? ContactPersonPhone { get; set; }
        public string? ContactPersonFaxNumber { get; set; }
        public string? ContactPersonJobTitle { get; set; }
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
        public Guid? BillingPlanId { get; set; }
        public string? BillingPlanName { get; set; }
        public AgencyWhitelabelResponse? AgencyWhitelabel { get; set; }
    }
}


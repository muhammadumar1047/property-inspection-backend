using System;
using System.Text.Json.Serialization;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class TenantDto : BaseEntityDto
    {
        public Guid TenancyId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? RentReviewDate { get; set; }
        public string? RentReviewNotes { get; set; }
        [JsonIgnore]
        public TenancyDto? Tenancy { get; set; }
    }
}

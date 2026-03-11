using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class UserDtoTemo : BaseEntityDto
    {
        public string IdentityUserId { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public string? ProfileImage { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAgencyAdmin { get; set; }
        public Guid AgencyId { get; set; }
        public AgencyDto? Agency { get; set; }
    }
}

using PropertyInspection.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.Auth
{
    public class UserResponse : BaseEntity
    {
        public string IdentityUserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; }
        public string ProfileImage { get; set; } = string.Empty;
        public Guid? AgencyId { get; set; }
        public string? AgencyName { get; set; } 
        public bool IsSuperAdmin { get; set; } = false;
        public bool IsAgencyAdmin { get; set; } = false;
        public List<UserRoleDto>? UserRoles { get; set; } = new List<UserRoleDto>();
    }
}


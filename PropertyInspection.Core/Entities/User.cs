using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class User : BaseEntity
    {
        public string? IdentityUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsSuperAdmin { get; set; } = false;
        public bool IsAgencyAdmin { get; set; } = false;
        public Guid? AgencyId { get; set; }
        public Agency? Agency { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

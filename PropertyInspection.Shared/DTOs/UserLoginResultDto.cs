using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class UserLoginResultDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid AgencyId { get; set; }
        public string? Username { get; set; }
        public string? ProfileImage { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

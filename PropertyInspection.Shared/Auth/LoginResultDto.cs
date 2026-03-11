using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.Auth
{
    public class LoginResultDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; }
        public Guid AgencyId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Message { get; set; } = "Login successful";
        public string? ProfileImage { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

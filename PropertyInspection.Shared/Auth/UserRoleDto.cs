using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.Auth
{
    public class UserRoleDto
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        //public List<PermissionDto>? Permissions { get; set; }
    }
}

using System;
using PropertyInspection.Core.Enums;
using PropertyInspection.Shared.Auth;

namespace PropertyInspection.Shared.DTOs
{
    public class RolePermissionDto
    {
        public Guid RoleId { get; set; }
        public RoleDto? Role { get; set; }
        public Guid PermissionId { get; set; }
        public PermissionDto? Permission { get; set; }
    }
}

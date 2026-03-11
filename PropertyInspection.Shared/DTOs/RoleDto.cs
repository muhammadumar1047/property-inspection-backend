using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public RoleType? RoleType { get; set; }
        public Guid? AgencyId { get; set; }
        public AgencyDto? Agency { get; set; }
    }
}

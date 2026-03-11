using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class LandlordDto : BaseEntityDto
    {
        public Guid PropertyId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public PropertyDto? Property { get; set; }
    }
}

using System;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Shared.DTOs
{
    public class TenancyDto : BaseEntityDto
    {
        public Guid PropertyId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Mobile { get; set; }
        public DateTime LeaseStartDate { get; set; }
        public DateTime LeaseEndDate { get; set; }
        public decimal CurrentRentAmount { get; set; }
        public RentFrequency RentFrequency { get; set; }
        public DateTime? OriginalLeaseDate { get; set; }
        public DateTime? TenantVacateDate { get; set; }
        public DateTime? NewInspectionDate { get; set; }
        public PropertyDto? Property { get; set; }
        public List<TenantDto> Tenants { get; set; } = new List<TenantDto>();
    }
}

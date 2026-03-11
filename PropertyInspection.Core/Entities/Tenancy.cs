using PropertyInspection.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class Tenancy : BaseEntity
    {
        [Required]
        public Guid PropertyId { get; set; }

        [Required, StringLength(200)]
        public string FullName { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string? Mobile { get; set; }

        [Required]
        public DateTime LeaseStartDate { get; set; }

        [Required]
        public DateTime LeaseEndDate { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal CurrentRentAmount { get; set; }

        [Required]
        public RentFrequency RentFrequency { get; set; } = RentFrequency.Day; // Enum usage

        public DateTime? OriginalLeaseDate { get; set; }
        public DateTime? TenantVacateDate { get; set; }
        public DateTime? NewInspectionDate { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PropertyId))]
        public virtual Property Property { get; set; } = null!;

        public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
    }
}

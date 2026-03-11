using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class TenancySnapshot : BaseEntity
    {
        [Required]
        public Guid InspectionId { get; set; }

        [Required]
        public Guid TenancyId { get; set; }  // Original Tenancy ID

        [Required, StringLength(200)]
        public string TenantName { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string TenantEmail { get; set; }

        [StringLength(20)]
        public string? TenantPhone { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal RentAmount { get; set; }

        // Navigation property
        [ForeignKey(nameof(InspectionId))]
        public virtual Inspection Inspection { get; set; } = null!;
    }
}

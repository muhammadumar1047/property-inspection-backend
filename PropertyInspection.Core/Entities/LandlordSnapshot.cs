using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class LandlordSnapshot : BaseEntity
    {
        [Required]
        public Guid InspectionId { get; set; }

        [Required]
        public Guid LandlordId { get; set; } // Original landlord ID

        [Required, StringLength(200)]
        public string FullName { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        // Navigation property
        [ForeignKey(nameof(InspectionId))]
        public virtual Inspection Inspection { get; set; } = null!;
    }
}

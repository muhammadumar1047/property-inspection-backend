using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class Tenant : BaseEntity
    {
        [Required]
        public Guid TenancyId { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        public DateTime? RentReviewDate { get; set; }

        [StringLength(500)]
        public string? RentReviewNotes { get; set; }

        // Navigation property
        [ForeignKey(nameof(TenancyId))]
        public virtual Tenancy Tenancy { get; set; } = null!;
    }
}

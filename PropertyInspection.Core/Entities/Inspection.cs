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
    public class Inspection : BaseEntity
    {
        [Required]
        public Guid PropertyId { get; set; }

        [Required]
        public Guid AgencyId { get; set; }

        [Required]
        public Guid InspectorId { get; set; }

        [Required]
        public InspectionType InspectionType { get; set; } = InspectionType.Entry; // Enum

        [Required]
        public InspectionStatus InspectionStatus { get; set; } = InspectionStatus.Pending; // Enum

        [Required]
        public DateTime InspectionDate { get; set; }

        [Required]
        public TimeSpan InspectionTime { get; set; }

        /// <summary>
        /// Date when the inspection was completed.
        /// </summary>
        public DateTime? InspectionCompletedDate { get; set; }

        /// <summary>
        /// Date when the inspection report was closed.
        /// </summary>
        public DateTime? InspectionCloseDate { get; set; }

        /// <summary>
        /// URL of the signature image uploaded during close.
        /// </summary>
        public string? SignatureImageUrl { get; set; }

        /// <summary>
        /// Date when the signature was provided.
        /// </summary>
        public DateTime? SignatureDate { get; set; }

        /// <summary>
        /// S3 URL of the generated PDF report. Null if not yet generated.
        /// </summary>
        public string? PdfUrl { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(PropertyId))]
        public virtual Property Property { get; set; } = null!;

        [ForeignKey(nameof(AgencyId))]
        public virtual Agency Agency { get; set; } = null!;

        [ForeignKey(nameof(InspectorId))]
        public virtual User Inspector { get; set; } = null!;
        public virtual ICollection<LandlordSnapshot> LandlordSnapshots { get; set; } = new List<LandlordSnapshot>();
        public virtual ICollection<TenancySnapshot> TenancySnapshots { get; set; } = new List<TenancySnapshot>();
    }
}

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

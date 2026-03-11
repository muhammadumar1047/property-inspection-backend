using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class Report : BaseEntity
    {
        [Required]
        public Guid InspectionId { get; set; }

        public Guid AgencyId { get; set; }

        [Required, StringLength(100)]
        public string ReportType { get; set; } = null!;

        [StringLength(2000)]
        public string? Notes { get; set; }

        // Navigation
        [ForeignKey(nameof(InspectionId))]
        public virtual Inspection Inspection { get; set; } = null!;

        public virtual ICollection<ReportArea> ReportAreas { get; set; } = new List<ReportArea>();
    }
}

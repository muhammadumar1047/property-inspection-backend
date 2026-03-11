using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class ReportItemCondition : BaseEntity
    {
        [Required]
        public Guid ReportItemId { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; } = null!;
            
        [StringLength(500)]
        public string? Value { get; set; }

        // Navigation
        [ForeignKey(nameof(ReportItemId))]
        public virtual ReportItem ReportItem { get; set; } = null!;
    }
}

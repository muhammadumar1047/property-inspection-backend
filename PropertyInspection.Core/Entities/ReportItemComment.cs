using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class ReportItemComment : BaseEntity
    {
        [Required]
        public Guid ReportItemId { get; set; }

        [Required, StringLength(1000)]
        public string Text { get; set; } = null!;

        // Navigation
        [ForeignKey(nameof(ReportItemId))]
        public virtual ReportItem ReportItem { get; set; } = null!;
    }
}

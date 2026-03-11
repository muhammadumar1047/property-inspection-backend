using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class ReportMediaComment : BaseEntity
    {
        [Required]
        public Guid ReportMediaId { get; set; }

        [Required, StringLength(1000)]
        public string Text { get; set; } = null!;

        public decimal? X { get; set; }
        public decimal? Y { get; set; }

        // Navigation
        [ForeignKey(nameof(ReportMediaId))]
        public virtual ReportMedia ReportMedia { get; set; } = null!;
    }
}

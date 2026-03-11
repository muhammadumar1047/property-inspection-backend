using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class ReportItem : BaseEntity
    {
        [Required]
        public Guid ReportAreaId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        // Navigation
        [ForeignKey(nameof(ReportAreaId))]
        public virtual ReportArea ReportArea { get; set; } = null!;

        public virtual ICollection<ReportItemCondition> ReportItemConditions { get; set; } = new List<ReportItemCondition>();
        public virtual ICollection<ReportItemComment> ReportItemComments { get; set; } = new List<ReportItemComment>();
        public virtual ICollection<ReportMedia> ReportMedia { get; set; } = new List<ReportMedia>();
    }
}

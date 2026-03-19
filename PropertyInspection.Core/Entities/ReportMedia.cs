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
    public class ReportMedia : BaseEntity
    {
        [Required]
        public Guid ReportItemId { get; set; }

        [Required, StringLength(2000)]
        public string Url { get; set; } = null!;

        [Required]
        public string Type { get; set; } = "";

        // Navigation
        [ForeignKey(nameof(ReportItemId))]
        public virtual ReportItem ReportItem { get; set; } = null!;

        public virtual ICollection<ReportMediaComment> ReportMediaComments { get; set; } = new List<ReportMediaComment>();
    }
}

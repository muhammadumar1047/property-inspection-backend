using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class ReportArea : BaseEntity
    {
        [Required]
        public Guid ReportId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        [ForeignKey(nameof(ReportId))]
        public virtual Report Report { get; set; } = null!;
        public virtual ICollection<ReportItem> ReportItems { get; set; } = new List<ReportItem>();
    }
}

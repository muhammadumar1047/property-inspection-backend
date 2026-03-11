using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class LayoutItem : BaseEntity
    {
        [Required]
        public Guid AreaId { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        public virtual LayoutArea Area { get; set; } = null!;
    }
}

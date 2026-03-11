using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class LayoutArea : BaseEntity
    {

        [Required]
        public Guid LayoutId { get; set; }

        [Required]
        [StringLength(100)]
        public string AreaName { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual PropertyLayout Layout { get; set; } = null!;
        public virtual ICollection<LayoutItem> Items { get; set; } = new List<LayoutItem>();
    }
}

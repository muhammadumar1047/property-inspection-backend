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
    public class PropertyLayout : BaseEntity
    {
        [Required]
        public Guid AgencyId { get; set; }

        [Required]
        public PropertyType LayoutType { get; set; } 

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public int DisplayOrder { get; set; }

        // Navigation properties
        [ForeignKey(nameof(AgencyId))]
        public virtual Agency Agency { get; set; } = null!;

        public virtual ICollection<LayoutArea> Areas { get; set; } = new List<LayoutArea>();
    }
}

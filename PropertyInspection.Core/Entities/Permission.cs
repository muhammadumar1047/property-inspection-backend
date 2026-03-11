using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class Permission : BaseEntity
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = null!;  

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Module { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

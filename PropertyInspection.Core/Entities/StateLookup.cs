using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class StateLookup
    {
        public Guid Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        public string? ShortCode { get; set; } 

        public Guid CountryId { get; set; }
        public virtual CountryLookup Country { get; set; } = null!;

        public virtual ICollection<Agency> Agencies { get; set; } = new List<Agency>();

    }
}

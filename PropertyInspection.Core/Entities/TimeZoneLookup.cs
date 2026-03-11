using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class TimeZoneLookup
    {
        public Guid Id { get; set; }

        [Required, StringLength(150)]
        public string DisplayName { get; set; } = string.Empty; 

        [Required, StringLength(100)]
        public string TimeZoneId { get; set; } = string.Empty; 

        public Guid? CountryId { get; set; }
        public virtual CountryLookup? Country { get; set; }

        public virtual ICollection<Agency> Agencies { get; set; } = new List<Agency>();
    }
}

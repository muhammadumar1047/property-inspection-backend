using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class CountryLookup
    {
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(3)]
        public string IsoAlpha3 { get; set; } = string.Empty;

        [Required, StringLength(2)]
        public string IsoAlpha2 { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<StateLookup> States { get; set; } = new List<StateLookup>();
        public virtual ICollection<TimeZoneLookup> TimeZones { get; set; } = new List<TimeZoneLookup>();

        public virtual ICollection<Agency> Agencies { get; set; } = new List<Agency>();
    }
}

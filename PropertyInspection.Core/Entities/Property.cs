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
    public class Property : BaseEntity
    {
        [Required]
        public Guid AgencyId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public PropertyType Type { get; set; }

        [Required]
        public Guid PropertyManagerId { get; set; }

        [Required, StringLength(200)]
        public string? Address1 { get; set; }

        [StringLength(200)]
        public string? Address2 { get; set; }

        [Required, StringLength(100)]
        public string CityOrSuburb { get; set; }

        [Required]
        public Guid StateLookupId { get; set; }

        [Required, StringLength(10)]
        public string Postcode { get; set; }

        [Required]
        public InspectionFrequencyType InspectionFrequencyType { get; set; }
        public int InspectionFrequencyNumber { get; set; }

        [StringLength(50)]
        public string? KeyNo { get; set; }

        [StringLength(50)]
        public string? AlarmCode { get; set; }

        [StringLength(1000)]
        public string? PropertyNotes { get; set; }

        public string? PropertyImages { get; set; }

        public Guid PropertyLayoutId { get; set; } 

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [ForeignKey(nameof(AgencyId))]
        public virtual Agency Agency { get; set; } = null!;

        [ForeignKey(nameof(PropertyManagerId))]
        public virtual User PropertyManager { get; set; } = null!;

        [ForeignKey(nameof(StateLookupId))]
        public virtual StateLookup State { get; set; } = null!;

        [ForeignKey(nameof(PropertyLayoutId))]
        public virtual PropertyLayout? PropertyLayout { get; set; }

        public virtual ICollection<Landlord> Landlords { get; set; } = new List<Landlord>();
        public virtual ICollection<Tenancy> Tenancies { get; set; } = new List<Tenancy>();
        public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    }
}

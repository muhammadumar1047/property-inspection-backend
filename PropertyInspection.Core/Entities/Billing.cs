using System.ComponentModel.DataAnnotations;

namespace PropertyInspection.Core.Entities
{
    public class Billing : BaseEntity
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PriceMonthly { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PriceYearly { get; set; }

        public int UserLimits { get; set; }

        public int TrialDays { get; set; }

        public int? PropertiesLimit { get; set; }

        public int? InspectionsLimit { get; set; }

        public ICollection<BillingFeature> Features { get; set; } = new List<BillingFeature>();
    }
}

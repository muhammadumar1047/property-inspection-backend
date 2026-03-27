using System.ComponentModel.DataAnnotations;

namespace PropertyInspection.Core.Entities
{
    public class BillingFeature
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BillingId { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; } = string.Empty;

        public Billing? Billing { get; set; }
    }
}

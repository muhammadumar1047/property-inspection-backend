using PropertyInspection.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyInspection.Core.Entities
{
    public class QuickSuggestion : BaseEntity
    {
        [Required]
        public Guid AgencyId { get; set; }

        [Required]
        public QuickSuggestionType Type { get; set; }

        [Required, StringLength(1000)]
        public string Text { get; set; } = null!;

        [StringLength(50)]
        public string? Shortcut { get; set; }

        [ForeignKey(nameof(AgencyId))]
        public virtual Agency Agency { get; set; } = null!;
    }
}

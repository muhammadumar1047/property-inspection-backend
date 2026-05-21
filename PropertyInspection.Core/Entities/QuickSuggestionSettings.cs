using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyInspection.Core.Entities
{
    public class QuickSuggestionSettings : BaseEntity
    {
        [Required]
        public Guid AgencyId { get; set; }

        public bool IsEntryExitEnabled { get; set; } = true;

        public bool IsRoutineEnabled { get; set; } = true;

        public bool CombineDictionaries { get; set; } = true;

        [ForeignKey(nameof(AgencyId))]
        public virtual Agency Agency { get; set; } = null!;
    }
}

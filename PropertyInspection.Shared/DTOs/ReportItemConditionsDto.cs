using System;

namespace PropertyInspection.Shared.DTOs
{
    /// <summary>
    /// Legacy fixed-condition DTO. Prefer a list of ReportItemConditionDto for flexible values.
    /// </summary>
    [Obsolete("Use a list of ReportItemConditionDto instead for flexible conditions.")]
    public class ReportItemConditionsDto
    {
        /// <summary>
        /// Indicates whether the item is clean.
        /// </summary>
        public bool Clean { get; set; }

        /// <summary>
        /// Indicates whether the item is undamaged.
        /// </summary>
        public bool Undamaged { get; set; }

        /// <summary>
        /// Indicates whether the item is working.
        /// </summary>
        public bool Working { get; set; }
    }
}

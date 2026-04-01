using System;
using System.Text.Json.Serialization;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportItemConditionDto
    {
        /// <summary>
        /// Unique identifier of the condition record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identifier of the parent report item.
        /// </summary>
        public Guid ReportItemId { get; set; }

        /// <summary>
        /// Condition label or description (e.g., "Clean", "Functional", "Notes").
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Condition type or category as stored with the report item.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Condition value stored as a string to allow flexible values.
        /// </summary>
        public string? Value { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Optional back-reference to the parent report item (ignored in JSON).
        /// </summary>
        public ReportItemDto? ReportItem { get; set; }
    }
}

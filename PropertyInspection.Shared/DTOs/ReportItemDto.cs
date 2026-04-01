using System;
using System.Collections.Generic;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportItemDto
    {
        /// <summary>
        /// Unique identifier of the report item.
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// Display name of the item as shown in the report.
        /// </summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// Flexible list of conditions captured for this item (not limited to fixed keys).
        /// </summary>
        public List<ReportItemConditionDto> Conditions { get; set; } = new();

        /// <summary>
        /// Combined inspector comments for this item.
        /// </summary>
        public string InspectorComments { get; set; } = string.Empty;

        /// <summary>
        /// All media associated with this item (photos and videos are not separated).
        /// </summary>
        public List<ReportMediaDto> Media { get; set; } = new();
    }
}

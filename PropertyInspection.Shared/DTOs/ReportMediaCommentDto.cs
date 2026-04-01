using System;
using System.Collections.Generic;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportMediaCommentDto
    {
        /// <summary>
        /// Unique identifier of the media comment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identifier of the parent media record.
        /// </summary>
        public Guid ReportMediaId { get; set; }

        /// <summary>
        /// Comment text entered by the inspector.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Optional X coordinate for an annotation point.
        /// </summary>
        public decimal? X { get; set; }

        /// <summary>
        /// Optional Y coordinate for an annotation point.
        /// </summary>
        public decimal? Y { get; set; }

        /// <summary>
        /// Optional list of agencies allowed to view this comment.
        /// </summary>
        public List<Guid> AgencyWhitelist { get; set; } = new();
    }
}

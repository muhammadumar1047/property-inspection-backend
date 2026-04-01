using System;
using System.Collections.Generic;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportMediaDto
    {
        /// <summary>
        /// Unique identifier of the media record.
        /// </summary>
        public Guid MediaId { get; set; }

        /// <summary>
        /// Identifier of the parent report item.
        /// </summary>
        public Guid ReportItemId { get; set; }

        /// <summary>
        /// Public URL of the media file.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Media type label stored with the record (e.g., "photo", "video").
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Comments attached to this media record.
        /// </summary>
        public List<ReportMediaCommentDto> Comments { get; set; } = new();
    }
}

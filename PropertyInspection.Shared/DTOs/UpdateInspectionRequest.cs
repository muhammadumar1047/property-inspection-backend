using PropertyInspection.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace PropertyInspection.Shared.DTOs
{
    public class UpdateInspectionRequest
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? AgencyId { get; set; }

        [Required]
        public Guid PropertyId { get; set; }

        [Required]
        public InspectionType InspectionType { get; set; }

        [Required]
        public InspectionStatus InspectionStatus { get; set; }

        [Required]
        public Guid InspectorId { get; set; }

        public string? Address { get; set; }

        [Required]
        public DateTime InspectionDate { get; set; }

        [Required]
        public TimeSpan InspectionTime { get; set; }

        /// <summary>
        /// Date when the inspection was completed.
        /// </summary>
        public DateTime? InspectionCompletedDate { get; set; }

        /// <summary>
        /// Date when the inspection report was closed.
        /// </summary>
        public DateTime? InspectionCloseDate { get; set; }

        /// <summary>
        /// URL of the signature image uploaded during close.
        /// </summary>
        public string? SignatureImageUrl { get; set; }

        /// <summary>
        /// Date when the signature was provided.
        /// </summary>
        public DateTime? SignatureDate { get; set; }
    }
}

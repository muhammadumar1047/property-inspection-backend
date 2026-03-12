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
    }
}

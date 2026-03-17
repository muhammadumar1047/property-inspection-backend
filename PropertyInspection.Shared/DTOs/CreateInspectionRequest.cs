using PropertyInspection.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class CreateInspectionRequest
    {
        [Required]
        public Guid PropertyId { get; set; }

        public Guid? AgencyId { get; set; }

        [Required]
        public InspectionType InspectionType { get; set; }

        [Required]
        public InspectionStatus InspectionStatus { get; set; }

        [Required]
        public Guid InspectorId { get; set; }

        //[Required]
        //public string Address { get; set; } = null!;

        [Required]
        public DateTime InspectionDate { get; set; }

        [Required]
        public TimeSpan InspectionTime { get; set; }
    }
}


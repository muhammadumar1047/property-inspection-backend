using PropertyInspection.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportInspectionResponse
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public Guid PropertyId { get; set; }
        public string PropertyAddress { get; set; } = string.Empty;
        public string PropertySubhurb { get; set; } = string.Empty;
        public Guid InspectorId { get; set; }
        public string InspectorName { get; set; }
        public InspectionType InspectionType { get; set; }
        public InspectionStatus InspectionStatus { get; set; }
        public DateTime InspectionDate { get; set; }
        public TimeSpan InspectionTime { get; set; }


    }
}

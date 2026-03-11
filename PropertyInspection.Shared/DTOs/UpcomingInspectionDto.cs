using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class UpcomingInspectionDto
    {
        public string PropertyAddress { get; set; } = string.Empty;
        public string InspectorName { get; set; } = string.Empty;
        public DateTime ScheduledDateTime { get; set; }
    }
}

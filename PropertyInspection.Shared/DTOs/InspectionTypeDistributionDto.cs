using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class InspectionTypeDistributionDto
    {
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

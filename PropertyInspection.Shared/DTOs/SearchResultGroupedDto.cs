using PropertyInspection.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class SearchResultDto
    {
        public string Type { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Subhurb { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string LandlordName { get; set; } = string.Empty;
        public DateTime? InspectionDate { get; set; }
        public InspectionType InspectionType { get; set; }
    }

    public class SearchResultGroupedDto
    {
        public List<SearchResultDto> Properties { get; set; } = new();
        public List<SearchResultDto> Inspections { get; set; } = new();
    }
}

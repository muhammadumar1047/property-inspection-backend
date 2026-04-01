using System;

namespace PropertyInspection.Shared.DTOs
{
    public class ReportHeaderDto
    {
        public Guid ReportId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string ReportTitle { get; set; } = string.Empty;
        public string AgencyLogoUrl { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public string AgencyPhone { get; set; } = string.Empty;
        public WhitelabelBrandingDto AgencyWhiteLabel { get; set; } = new();
        public string PropertyAddress { get; set; } = string.Empty;
        public DateTime? LeaseStartDate { get; set; }
        public DateTime? LeaseEndDate { get; set; }
        public DateTime InspectionDate { get; set; }
        public ReportTenantDto Tenant { get; set; } = new();
        public ReportInspectorDto Inspector { get; set; } = new();
    }
}

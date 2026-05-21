using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class EmailTemplateProcessor : IEmailTemplateProcessor
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmailTemplateProcessor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> ProcessTemplateAsync(string templateBody, Guid inspectionId)
        {
            if (string.IsNullOrWhiteSpace(templateBody))
            {
                return string.Empty;
            }

            var inspection = await _unitOfWork.Inspections.FirstOrDefaultAsync(
                i => i.Id == inspectionId,
                include: q => q
                    .Include(i => i.Property)
                    .Include(i => i.Agency)
                    .Include(i => i.TenancySnapshots));

            if (inspection == null)
            {
                return templateBody;
            }

            var tenantName = inspection.TenancySnapshots?.FirstOrDefault()?.TenantName ?? "Tenant";
            var propertyAddress = BuildPropertyAddress(inspection.Property);
            var inspectionDate = inspection.InspectionDate.ToString("dddd, dd MMMM yyyy");
            var inspectionType = inspection.InspectionType.ToString() + " Inspection";
            var officeName = inspection.Agency?.LegalBusinessName ?? "EaseInspect Office";
            var reportLink = $"https://app.easeinspect.com/reports/preview-{inspection.Id}";

            var processed = templateBody
                .Replace("%TenantFullName%", tenantName)
                .Replace("%PropertyAddress%", propertyAddress)
                .Replace("%InspectionDate%", inspectionDate)
                .Replace("%InspectionType%", inspectionType)
                .Replace("%OfficeName%", officeName)
                .Replace("%InspectionReportLink%", reportLink);

            return processed;
        }

        private static string BuildPropertyAddress(Core.Entities.Property property)
        {
            if (property == null) return string.Empty;
            var parts = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(property.Address1)) parts.Add(property.Address1.Trim());
            if (!string.IsNullOrWhiteSpace(property.Address2)) parts.Add(property.Address2.Trim());
            if (!string.IsNullOrWhiteSpace(property.CityOrSuburb)) parts.Add(property.CityOrSuburb.Trim());
            if (!string.IsNullOrWhiteSpace(property.Postcode)) parts.Add(property.Postcode.Trim());
            return string.Join(", ", parts);
        }
    }
}

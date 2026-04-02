using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;
using PropertyInspection.Core.Entities;

namespace PropertyInspection.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<ReportDto>> GetReportByInspectionIdAsync(Guid inspectionId , Guid? agencyId)
        {
            try
            {
                var report = await _unitOfWork.Reports.FirstOrDefaultAsync(
                    r => r.InspectionId == inspectionId && (agencyId == null || r.AgencyId == agencyId),
                    include: q => q
                        .AsNoTracking()
                        .Include(r => r.Inspection)
                            .ThenInclude(i => i.Property)
                        .Include(r => r.Inspection)
                            .ThenInclude(i => i.Agency)
                                .ThenInclude(a => a.AgencyWhitelabel)
                        .Include(r => r.Inspection)
                            .ThenInclude(i => i.Inspector)
                        .Include(r => r.Inspection)
                            .ThenInclude(i => i.LandlordSnapshots)
                        .Include(r => r.Inspection)
                            .ThenInclude(i => i.TenancySnapshots)
                        .Include(r => r.ReportAreas)
                            .ThenInclude(area => area.ReportItems)
                                .ThenInclude(item => item.ReportItemConditions)
                        .Include(r => r.ReportAreas)
                            .ThenInclude(area => area.ReportItems)
                                .ThenInclude(item => item.ReportItemComments)
                        .Include(r => r.ReportAreas)
                            .ThenInclude(area => area.ReportItems)
                                .ThenInclude(item => item.ReportMedia)
                                    .ThenInclude(media => media.ReportMediaComments));

                if (report == null)
                {
                    return new ServiceResponse<ReportDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                var reportDto = MapReport(report);

                return new ServiceResponse<ReportDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = reportDto
                };
            }
            catch
            {
                return new ServiceResponse<ReportDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        private ReportDto MapReport(Report report)
        {
            var inspection = report.Inspection;
            var agency = inspection?.Agency;
            var whitelabel = agency?.AgencyWhitelabel;
            var tenancy = inspection?.TenancySnapshots?
                .OrderByDescending(t => t.StartDate)
                .FirstOrDefault();

            var tenantContactParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(tenancy?.TenantEmail))
            {
                tenantContactParts.Add(tenancy.TenantEmail.Trim());
            }
            if (!string.IsNullOrWhiteSpace(tenancy?.TenantPhone))
            {
                tenantContactParts.Add(tenancy.TenantPhone.Trim());
            }

            var inspectorName = inspection?.Inspector == null
                ? string.Empty
                : string.Join(" ", new[] { inspection.Inspector.FirstName, inspection.Inspector.LastName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            var propertyAddressParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(inspection?.Property?.Address1))
            {
                propertyAddressParts.Add(inspection.Property.Address1!.Trim());
            }
            if (!string.IsNullOrWhiteSpace(inspection?.Property?.Address2))
            {
                propertyAddressParts.Add(inspection.Property.Address2!.Trim());
            }
            if (!string.IsNullOrWhiteSpace(inspection?.Property?.CityOrSuburb))
            {
                propertyAddressParts.Add(inspection.Property.CityOrSuburb!.Trim());
            }
            if (!string.IsNullOrWhiteSpace(inspection?.Property?.Postcode))
            {
                propertyAddressParts.Add(inspection.Property.Postcode!.Trim());
            }

            var header = new ReportHeaderDto
            {
                ReportId = report.Id,
                ReportType = report.ReportType ?? string.Empty,
                ReportTitle = BuildReportTitle(report.ReportType),
                AgencyLogoUrl = whitelabel?.LogoUrl ?? string.Empty,
                AgencyName = agency?.LegalBusinessName ?? string.Empty,
                AgencyPhone = agency?.PhoneNumber ?? string.Empty,
                AgencyWhiteLabel = whitelabel == null
                    ? new WhitelabelBrandingDto()
                    : new WhitelabelBrandingDto
                    {
                        AgencyNameColor = whitelabel.AgencyNameColor ?? string.Empty,
                        AddressColor = whitelabel.AddressColor ?? string.Empty,
                        AccentColor = whitelabel.AccentColor,
                        AccentFontFamily = whitelabel.AccentFontFamily,
                        LogoUrl = whitelabel.LogoUrl,
                        PrimaryColor = whitelabel.PrimaryColor,
                        SecondaryColor = whitelabel.SecondaryColor,
                        FontFamily = whitelabel.FontFamily
                    },
                PropertyAddress = string.Join(", ", propertyAddressParts),
                LeaseStartDate = tenancy?.StartDate,
                LeaseEndDate = tenancy?.EndDate,
                InspectionDate = inspection?.InspectionDate ?? DateTime.MinValue,
                Tenant = new ReportTenantDto
                {
                    Name = tenancy?.TenantName ?? string.Empty,
                    ContactInfo = string.Join(" | ", tenantContactParts)
                },
                Inspector = new ReportInspectorDto
                {
                    Name = inspectorName,
                    Email = inspection?.Inspector?.Email ?? string.Empty
                }
            };

            var areas = report.ReportAreas?.Select(area => new ReportAreaDto
            {
                AreaId = area.Id,
                AreaName = area.Name ?? string.Empty,
                Items = area.ReportItems?.Select(item => MapReportItem(item, report.AgencyId)).ToList() ?? new List<ReportItemDto>()
            }).ToList() ?? new List<ReportAreaDto>();

            return new ReportDto
            {
                Id = report.Id,
                CreatedAt = report.CreatedAt,
                CreatedBy = report.CreatedBy,
                UpdatedAt = report.UpdatedAt,
                UpdatedBy = report.UpdatedBy,
                IsDeleted = report.IsDeleted,
                DeletedAt = report.DeletedAt,
                DeletedBy = report.DeletedBy,
                IsActive = report.IsActive,
                InspectionId = report.InspectionId,
                ReportType = report.ReportType ?? string.Empty,
                Notes = report.Notes ?? string.Empty,
                Inspection = inspection == null ? null : _mapper.Map<InspectionResponse>(inspection),
                Header = header,
                Areas = areas
            };
        }

        private static ReportItemDto MapReportItem(ReportItem item, Guid reportAgencyId)
        {
            var conditions = item.ReportItemConditions ?? new List<ReportItemCondition>();

            var commentTexts = item.ReportItemComments?
                .Select(c => c.Text)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t!.Trim())
                .ToList() ?? new List<string>();

            var media = item.ReportMedia ?? new List<ReportMedia>();

            var conditionDtos = conditions
                .Select(c => new ReportItemConditionDto
                {
                    Id = c.Id,
                    ReportItemId = c.ReportItemId,
                    Description = c.Description ?? string.Empty,
                    Type = c.Type ?? string.Empty,
                    Value = c.Value
                })
                .ToList();

            var mediaDtos = media
                .Select(m => new ReportMediaDto
                {
                    MediaId = m.Id,
                    ReportItemId = m.ReportItemId,
                    Url = m.Url ?? string.Empty,
                    Type = m.Type ?? string.Empty,
                    Comments = (m.ReportMediaComments ?? new List<ReportMediaComment>())
                        .Select(c => new ReportMediaCommentDto
                        {
                            Id = c.Id,
                            ReportMediaId = c.ReportMediaId,
                            Text = c.Text ?? string.Empty,
                            X = c.X,
                            Y = c.Y,
                            //AgencyWhitelist = reportAgencyId == Guid.Empty
                            //    ? new List<Guid>()
                            //    : new List<Guid> { reportAgencyId }
                        })
                        .ToList()
                })
                .ToList();

            return new ReportItemDto
            {
                ItemId = item.Id,
                ItemName = item.Name ?? string.Empty,
                Conditions = conditionDtos,
                InspectorComments = commentTexts.Count == 0 ? string.Empty : string.Join("\n", commentTexts),
                Media = mediaDtos
            };
        }

        private static string BuildReportTitle(string? reportType)
        {
            if (string.IsNullOrWhiteSpace(reportType))
            {
                return "Condition Report";
            }

            if (string.Equals(reportType, "Entry", StringComparison.OrdinalIgnoreCase))
            {
                return "Entry Condition Report";
            }

            if (string.Equals(reportType, "Exit", StringComparison.OrdinalIgnoreCase))
            {
                return "Exit Condition Report";
            }

            if (string.Equals(reportType, "Routine", StringComparison.OrdinalIgnoreCase))
            {
                return "Routine Inspection Report";
            }

            return $"{reportType.Trim()} Report";
        }
    }
}

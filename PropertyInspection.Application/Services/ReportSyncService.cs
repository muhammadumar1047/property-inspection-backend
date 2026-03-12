using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.Services
{
    public class ReportSyncService : IReportSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;


        public ReportSyncService(IUnitOfWork unitOfWork, ITenantAgencyResolver tenantAgencyResolver)
        {
            _unitOfWork = unitOfWork;
            _tenantAgencyResolver = tenantAgencyResolver;

        }

        public async Task<ServiceResponse<bool>> SyncReportAsync(ReportSyncDto reportDto)
        {
            try
            {
                if (reportDto == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(reportDto.AgencyId);

                var report = new Report
                {
                    Id = reportDto.ReportId,
                    AgencyId = tenantAgencyId,
                    InspectionId = reportDto.InspectionId,
                    ReportType = reportDto.ReportType,
                    Notes = reportDto.Notes,
                    ReportAreas = reportDto.ReportAreas?.Select(areaDto => new ReportArea
                    {
                        Name = areaDto.Name,
                        ReportItems = areaDto.ReportItems?.Select(itemDto => new ReportItem
                        {
                            Name = itemDto.Name,
                            ReportItemConditions = itemDto.ReportItemConditions?.Select(c => new ReportItemCondition
                            {
                                Description = c.Description,
                                Value = c.Value
                            }).ToList() ?? new List<ReportItemCondition>(),
                            ReportItemComments = itemDto.ReportItemComments?.Select(c => new ReportItemComment
                            {
                                Text = c.Text
                            }).ToList() ?? new List<ReportItemComment>(),
                            ReportMedia = itemDto.ReportMedia?.Select(m => new ReportMedia
                            {
                                Url = m.Url,
                                Type = m.Type,
                                ReportMediaComments = m.ReportMediaComments?.Select(mc => new ReportMediaComment
                                {
                                    Text = mc.Text,
                                    X = mc.X,
                                    Y = mc.Y
                                }).ToList() ?? new List<ReportMediaComment>()
                            }).ToList() ?? new List<ReportMedia>()
                        }).ToList() ?? new List<ReportItem>()
                    }).ToList() ?? new List<ReportArea>()
                };

                var success = await _unitOfWork.ReportSync.SyncReportAsync(report);

                if (success)
                {
                    var inspection = await _unitOfWork.Inspections
                        .FirstOrDefaultAsync(i => i.Id == report.InspectionId);

                    if (inspection != null)
                    {
                        inspection.InspectionStatus = Core.Enums.InspectionStatus.Completed;
                        await _unitOfWork.Inspections.UpdateAsync(inspection);
                        await _unitOfWork.CommitAsync();
                    }
                }

                return new ServiceResponse<bool>
                {
                    Success = success,
                    Message = success ? "Record updated successfully" : "Unable to process the request at the moment",
                    ErrorCode = success ? null : ServiceErrorCodes.ServerError,
                    Data = success
                };
            }
            catch
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.Factories;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.Services
{
    public class ReportTemplateService : IReportTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<ReportTemplateDto>> GenerateReportTemplateForPCR(Guid inspectionId)
        {
            try
            {
                var inspection = await _unitOfWork.Inspections
                    .FirstOrDefaultAsync(i => i.Id == inspectionId);

                if (inspection == null)
                {
                    return new ServiceResponse<ReportTemplateDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                var property = await _unitOfWork.Properties.FirstOrDefaultAsync(
                    p => p.Id == inspection.PropertyId,
                    include: query => query
                        .Include(p => p.PropertyLayout)
                            .ThenInclude(pl => pl.Areas)
                                .ThenInclude(a => a.Items)
                );

                if (property == null)
                {
                    return new ServiceResponse<ReportTemplateDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<ReportTemplateDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = BuildReportTemplateForPCR(property, inspection)
                };
            }
            catch
            {
                return new ServiceResponse<ReportTemplateDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<ReportTemplateDto>> GenerateReportTemplateForRoutine(Guid inspectionId)
        {
            try
            {
                var inspection = await _unitOfWork.Inspections
                    .FirstOrDefaultAsync(i => i.Id == inspectionId);

                if (inspection == null)
                {
                    return new ServiceResponse<ReportTemplateDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                var property = await _unitOfWork.Properties.FirstOrDefaultAsync(
                    p => p.Id == inspection.PropertyId,
                    include: query => query
                        .Include(p => p.PropertyLayout)
                            .ThenInclude(pl => pl.Areas)
                                .ThenInclude(a => a.Items)
                );

                if (property == null)
                {
                    return new ServiceResponse<ReportTemplateDto>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<ReportTemplateDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = BuildReportTemplateForRoutine(property, inspection)
                };
            }
            catch
            {
                return new ServiceResponse<ReportTemplateDto>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        private ReportTemplateDto BuildReportTemplateForPCR(Property property, Inspection inspection)
        {
            var template = new ReportTemplateDto
            {
                ReportId = Guid.Empty,
                InspectionId = inspection.Id,
                ReportType = "Ingoing",
                Notes = "",
                ReportAreas = new List<ReportTemplateAreaDto>()
            };

            template.ReportAreas.Add(ReportTemplateFactory.GenerateUtilitiesAreaForPCR());

            if (property.PropertyLayout?.Areas != null)
            {
                foreach (var layoutArea in property.PropertyLayout.Areas)
                {
                    var areaDto = new ReportTemplateAreaDto
                    {
                        ReportAreaId = Guid.Empty,
                        Name = layoutArea.AreaName,
                        ReportItems = layoutArea.Items.Select(item => new ReportTemplateItemDto
                        {
                            Name = item.ItemName,
                        ReportItemConditions = new List<ReportTemplateItemConditionDto>
                            {
                                new() { Description = "Clean", Value = "false" },
                                new() { Description = "Undamaged", Value = "false" },
                                new() { Description = "Working", Value = "false" }
                            },
                            ReportItemComments = new List<ReportTemplateItemCommentDto>
                            {
                                new() { Text = "" },
                            },
                            ReportMedia = new List<ReportTemplateMediaDto>
                            {
                                new() { Url = "", Type = "" },
                            }
                        }).ToList()
                    };

                    template.ReportAreas.Add(areaDto);
                }
            }

            return template;
        }

        private ReportTemplateDto BuildReportTemplateForRoutine(Property property, Inspection inspection)
        {
            var template = new ReportTemplateDto
            {
                ReportId = Guid.Empty,
                InspectionId = inspection.Id,
                ReportType = "Routine",
                Notes = "",
                ReportAreas = new List<ReportTemplateAreaDto>()
            };

            template.ReportAreas.Add(ReportTemplateFactory.GenerateRoutineInspectionSummaryAreaForRoutine());

            var routineArea = new ReportTemplateAreaDto
            {
                ReportAreaId = Guid.Empty,
                Name = "Routine Inspection",
                ReportItems = new List<ReportTemplateItemDto>()
            };

            if (property.PropertyLayout?.Areas != null)
            {
                foreach (var layoutArea in property.PropertyLayout.Areas)
                {
                    var childArea = new ReportTemplateItemDto
                    {
                        ReportItemId = Guid.Empty,
                        Name = layoutArea.AreaName,
                        ReportItemConditions = new List<ReportTemplateItemConditionDto>
                        {
                            new() { Description = "Condition Satisfactory", Value = "false" },
                            new() { Description = "Action required by tenant", Value = "false" },
                            new() { Description = "Action required by landlord", Value = "false" }
                        },
                        ReportItemComments = new List<ReportTemplateItemCommentDto>
                        {
                            new() { Text = "" },
                        },
                        ReportMedia = new List<ReportTemplateMediaDto>
                        {
                            new() { Url = "", Type = "" },
                        }
                    };

                    routineArea.ReportItems.Add(childArea);
                }
            }

            template.ReportAreas.Add(routineArea);
            return template;
        }
    }
}

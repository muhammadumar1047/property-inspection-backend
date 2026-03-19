using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.Factories;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.Services
{
    public class MobileReportTemplateService :IMobileReportTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MobileReportTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<ReportTemplateDto>> GenerateEntryExitTemplateAsync(Guid inspectionId)
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

                if (inspection.InspectionType != InspectionType.Entry && inspection.InspectionType != InspectionType.Exit)
                {
                    return new ServiceResponse<ReportTemplateDto>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
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

                var template = BuildEntryExitTemplate(property, inspection);

                return new ServiceResponse<ReportTemplateDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = template
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

        public async Task<ServiceResponse<ReportTemplateDto>> GenerateRoutineTemplateAsync(Guid inspectionId)
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

                var template = BuildRoutineTemplate(property, inspection);

                return new ServiceResponse<ReportTemplateDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = template
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

        private ReportTemplateDto BuildEntryExitTemplate(Property property, Inspection inspection)
        {
            var template = new ReportTemplateDto
            {
                ReportId = Guid.Empty,
                InspectionId = inspection.Id,
                ReportType = inspection.InspectionType.ToString(),
                Notes = string.Empty,
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
                                new() { Description = "Clean", Type = "boolean", Value = null },
                                new() { Description = "Undamaged", Type = "boolean", Value = null },
                                new() { Description = "Working", Type = "boolean", Value = null }
                            },
                            ReportItemComments = new List<ReportTemplateItemCommentDto>
                            {
                                new() { Text = string.Empty },
                            },
                            ReportMedia = CreateDefaultMedia()
                        }).ToList()
                    };

                    template.ReportAreas.Add(areaDto);
                }
            }

            EnsureMediaPlaceholders(template);
            return template;
        }

        private ReportTemplateDto BuildRoutineTemplate(Property property, Inspection inspection)
        {
            var template = new ReportTemplateDto
            {
                ReportId = Guid.Empty,
                InspectionId = inspection.Id,
                ReportType = "Routine",
                Notes = string.Empty,
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
                            new() { Description = "Condition Satisfactory", Type = "boolean", Value = null },
                            new() { Description = "Action required by tenant", Type = "boolean", Value = null },
                            new() { Description = "Action required by landlord", Type = "boolean", Value = null }
                        },
                        ReportItemComments = new List<ReportTemplateItemCommentDto>
                        {
                            new() { Text = string.Empty },
                        },
                        ReportMedia = CreateDefaultMedia()
                    };

                    routineArea.ReportItems.Add(childArea);
                }
            }

            template.ReportAreas.Add(routineArea);
            EnsureMediaPlaceholders(template);
            return template;
        }

        private static List<ReportTemplateMediaDto> CreateDefaultMedia()
        {
            return new List<ReportTemplateMediaDto>
            {
                new() { Url = string.Empty, Type = "photo" },
                new() { Url = string.Empty, Type = "video" }
            };
        }

        private static void EnsureMediaPlaceholders(ReportTemplateDto template)
        {
            foreach (var area in template.ReportAreas)
            {
                foreach (var item in area.ReportItems)
                {
                    if (item.ReportMedia == null || item.ReportMedia.Count < 2)
                    {
                        item.ReportMedia = CreateDefaultMedia();
                    }
                }
            }
        }
    }
}


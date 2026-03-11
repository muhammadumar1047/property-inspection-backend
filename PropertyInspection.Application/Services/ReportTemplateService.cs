using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.Factories;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.Services
{
    public class ReportTemplateService : IReportTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReportTemplateDto?> GenerateReportTemplateForPCR(Guid inspectionId)
        {
            var inspection = await _unitOfWork.Inspections
                .FirstOrDefaultAsync(i => i.Id == inspectionId);

            if (inspection == null)
                return null;

            var property = await _unitOfWork.Properties
                .FirstOrDefaultAsync(p => p.Id == inspection.PropertyId);

            if (property == null)
                throw new Exception("Property not found");

            return BuildReportTemplateForPCR(property, inspection);
        }

        public async Task<ReportTemplateDto?> GenerateReportTemplateForRoutine(Guid inspectionId)
        {
            var inspection = await _unitOfWork.Inspections
                .FirstOrDefaultAsync(i => i.Id == inspectionId);

            if (inspection == null)
                return null;

            var property = await _unitOfWork.Properties
                .FirstOrDefaultAsync(p => p.Id == inspection.PropertyId);

            if (property == null)
                throw new Exception("Property not found");

            return BuildReportTemplateForRoutine(property, inspection);
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

            template.ReportAreas.Add(ReportTemplateFactory.GenerateAdditionalChecksAreaForPCR());

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
                            ReportItemComments = new List<ReportTemplateItemCommentDto>(),
                            ReportMedia = new List<ReportTemplateMediaDto>()
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
                        ReportItemComments = new List<ReportTemplateItemCommentDto>(),
                        ReportMedia = new List<ReportTemplateMediaDto>()
                    };

                    routineArea.ReportItems.Add(childArea);
                }
            }

            template.ReportAreas.Add(routineArea);
            return template;
        }
    }
}

using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Factories
{
    public class ReportTemplateFactory
    {
        public static ReportTemplateAreaDto GenerateUtilitiesAreaForPCR()
        {

            //Property Condition Report (PCR) - Utilities Area Template
            return new ReportTemplateAreaDto
            {
                ReportAreaId = Guid.Empty,
                Name = "Utilities",
                ReportItems = new List<ReportTemplateItemDto>
            {
                new ReportTemplateItemDto
                {
                    Name = "Communication Facilities",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Telephone line connected", Type = "boolean", Value = null },
                        new() { Description = "Internet line connected", Type = "boolean", Value = null }
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Water Efficiency Devices",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Premises separately metered", Type = "boolean", Value = null },
                        new() { Description = "Showerheads max flow rate OK", Type = "boolean", Value = null },
                        new() { Description = "Dual flush toilets (3 star WELS)", Type = "boolean", Value = null },
                        new() { Description = "Cold water taps max flow 9 L/min", Type = "boolean", Value = null },
                        new() { Description = "Leaking taps/toilets fixed", Type = "boolean", Value = null },
                        new() { Description = "Water Meter Location", Type = "text", Value = null },
                        new() { Description = "Water Meter Reading", Type = "number", Value = null }
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Health Issues",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Signs of mould/dampness", Type = "boolean", Value = null },
                        new() { Description = "Pests or vermin present", Type = "boolean", Value = null },
                        new() { Description = "Rubbish left on premises", Type = "boolean", Value = null },
                        new() { Description = "Loose-Fill Asbestos listed", Type = "boolean", Value = null },
                        new() { Description = "Child safety devices installed", Type = "boolean", Value = null },
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Work Completed",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Installation of water efficiency measures", Type = "boolean", Value = null },
                        new() { Description = "Painting (internal)", Type = "boolean", Value = null },
                        new() { Description = "Painting (external)", Type = "boolean", Value = null },
                        new() { Description = "Flooring laid/replaced/cleaned", Type = "boolean", Value = null },
                        new() { Description = "Smoke alarm installation/repair", Type = "boolean", Value = null },
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Other Safety Issues",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Damaged appliances visible", Type = "boolean", Value = null },
                        new() { Description = "Electrical hazards", Type = "boolean", Value = null },
                        new() { Description = "Gas hazards", Type = "boolean", Value = null },
                        new() { Description = "Tenant agrees with Other Safety Issues", Type = "boolean", Value = null },
                        new() { Description = "Disagreement Details", Type = "text", Value = null },
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Smoke Alarm",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Installed as per law", Type = "boolean", Value = null },
                        new() { Description = "All alarms working", Type = "boolean", Value = null },
                        new() { Description = "Removable batteries replaced (12 months)", Type = "boolean", Value = null },
                        new() { Description = "Lithium batteries replaced as per manufacturer", Type = "boolean", Value = null },
                        new() { Description = "Last Checked Date", Type = "date", Value = null },
                        new() { Description = "Last Battery Change Date", Type = "date", Value = null },
                        new() { Description = "Smoke alarm comments", Type = "text", Value = null },
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Minimum Standards",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Structurally sound", Type = "boolean", Value = null },
                        new() { Description = "Adequate lighting", Type = "boolean", Value = null },
                        new() { Description = "Adequate ventilation", Type = "boolean", Value = null },
                        new() { Description = "Adequate power/gas outlets", Type = "boolean", Value = null },
                        new() { Description = "Adequate plumbing/drainage", Type = "boolean", Value = null },
                        new() { Description = "Supplied with electricity", Type = "boolean", Value = null },
                        new() { Description = "Supplied with gas", Type = "boolean", Value = null },
                        new() { Description = "Connected to water supply", Type = "boolean", Value = null },
                        new() { Description = "Bathroom privacy available", Type = "boolean", Value = null },
                        new() { Description = "Tenant agrees with Minimum Standards", Type = "boolean", Value = null },
                        new() { Description = "Disagreement Details", Type = "text", Value = null },
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Additional Comments",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Additional Comments", Type = "text", Value = null },
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Furniture List",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Furniture", Type = "text", Value = null },
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Custom",
                    ReportItemConditions = new List<ReportTemplateItemConditionDto>
                    {
                        new() { Description = "Custom Notes", Type = "text", Value = null },
                    },
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
            }
            };
        }


        public static ReportTemplateAreaDto GenerateRoutineInspectionSummaryAreaForRoutine()
        {

            return new ReportTemplateAreaDto
            {
                ReportAreaId = Guid.Empty,
                Name = "RoutineInspectionSummary",
                ReportItems = new List<ReportTemplateItemDto>
            {
                new ReportTemplateItemDto
                {
                    Name = "General Comments",
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Maintenance Required",
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Work to be Carried out by Tenant",
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Work to be Carried out by Landlord",
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Smoke Alarms",
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Rent Review",
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
                new ReportTemplateItemDto
                {
                    Name = "Pets at Property",
                    ReportItemComments = new List<ReportTemplateItemCommentDto>
                    {
                        new() { Text = "" },
                    },
                    ReportMedia = new List<ReportTemplateMediaDto>
                    {
                        new() { Url = "", Type = "" },
                    }
                },
            }
            };
        }
    }
}


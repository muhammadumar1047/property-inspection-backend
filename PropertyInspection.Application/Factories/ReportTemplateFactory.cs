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
                        new() { Description = "Telephone line connected", Value = "false" },
                        new() { Description = "Internet line connected", Value = "false" }
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
                        new() { Description = "Premises separately metered", Value = "false" },
                        new() { Description = "Showerheads max flow rate OK", Value = "false" },
                        new() { Description = "Dual flush toilets (3 star WELS)", Value = "false" },
                        new() { Description = "Cold water taps max flow 9 L/min", Value = "false" },
                        new() { Description = "Leaking taps/toilets fixed", Value = "false" },
                        new() { Description = "Water Meter Location", Value = "" },
                        new() { Description = "Water Meter Reading", Value = "" }
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
                        new() { Description = "Signs of mould/dampness", Value = "false" },
                        new() { Description = "Pests or vermin present", Value = "false" },
                        new() { Description = "Rubbish left on premises", Value = "false" },
                        new() { Description = "Loose-Fill Asbestos listed", Value = "false" },
                        new() { Description = "Child safety devices installed", Value = "false" },
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
                        new() { Description = "Installation of water efficiency measures", Value = "false" },
                        new() { Description = "Painting (internal)", Value = "false" },
                        new() { Description = "Painting (external)", Value = "false" },
                        new() { Description = "Flooring laid/replaced/cleaned", Value = "false" },
                        new() { Description = "Smoke alarm installation/repair", Value = "false" },
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
                        new() { Description = "Damaged appliances visible", Value = "false" },
                        new() { Description = "Electrical hazards", Value = "false" },
                        new() { Description = "Gas hazards", Value = "false" },
                        new() { Description = "Tenant agrees with Other Safety Issues", Value = "false" },
                        new() { Description = "Disagreement Details", Value = "false" },
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
                        new() { Description = "Installed as per law", Value = "false" },
                        new() { Description = "All alarms working", Value = "false" },
                        new() { Description = "Removable batteries replaced (12 months)", Value = "false" },
                        new() { Description = "Lithium batteries replaced as per manufacturer", Value = "false" },
                        new() { Description = "Last Checked Date", Value = "2025-09-14" },
                        new() { Description = "Last Battery Change Date", Value = "2025-06-20" },
                        new() { Description = "Smoke alarm comments", Value = "All alarms tested and functional." },
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
                        new() { Description = "Structurally sound", Value = "false" },
                        new() { Description = "Adequate lighting", Value = "false" },
                        new() { Description = "Adequate ventilation", Value = "false" },
                        new() { Description = "Adequate power/gas outlets", Value = "false" },
                        new() { Description = "Adequate plumbing/drainage", Value = "false" },
                        new() { Description = "Supplied with electricity", Value = "false" },
                        new() { Description = "Supplied with gas", Value = "false" },
                        new() { Description = "Connected to water supply", Value = "false" },
                        new() { Description = "Bathroom privacy available", Value = "false" },
                        new() { Description = "Tenant agrees with Minimum Standards", Value = "false" },
                        new() { Description = "Disagreement Details", Value = "false" },
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
                        new() { Description = "Additional Comments", Value = "" },
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
                        new() { Description = "Furniture", Value = "" },
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
                        new() { Description = "Custom Notes", Value = "" },
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


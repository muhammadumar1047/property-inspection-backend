using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyInspection.Shared.DTOs
{
    public class BillingFeatureDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class BillingDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PriceMonthly { get; set; }
        public decimal PriceYearly { get; set; }
        public string Status { get; set; } = "active";
        public string CreatedDate { get; set; } = string.Empty;
        public List<BillingFeatureDto> Features { get; set; } = new List<BillingFeatureDto>();
        public int UserLimits { get; set; }
        public int TrialDays { get; set; }
        public int? PropertiesLimit { get; set; }
        public int? InspectionsLimit { get; set; }
    }

    public class CreateBillingDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PriceMonthly { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PriceYearly { get; set; }

        [Required]
        public string Status { get; set; } = "active";

        public List<BillingFeatureDto> Features { get; set; } = new List<BillingFeatureDto>();

        public int UserLimits { get; set; }

        public int TrialDays { get; set; }

        public int? PropertiesLimit { get; set; }

        public int? InspectionsLimit { get; set; }
    }

    public class UpdateBillingDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PriceMonthly { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PriceYearly { get; set; }

        public string? Status { get; set; }

        public List<BillingFeatureDto>? Features { get; set; }

        public int? UserLimits { get; set; }

        public int? TrialDays { get; set; }

        public int? PropertiesLimit { get; set; }

        public int? InspectionsLimit { get; set; }
    }

    public class BillingFilterDto
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

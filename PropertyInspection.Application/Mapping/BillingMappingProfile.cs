using AutoMapper;
using PropertyInspection.Core.Entities;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.Mapping
{
    public class BillingMappingProfile : Profile
    {
        public BillingMappingProfile()
        {
            CreateMap<BillingFeature, BillingFeatureDto>();

            CreateMap<Billing, BillingDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsActive ? "active" : "inactive"))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.CreatedAt.ToString("yyyy-MM-dd")));
        }
    }
}

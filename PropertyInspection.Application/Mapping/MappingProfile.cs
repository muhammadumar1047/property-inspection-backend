using AutoMapper;
using System;
using System.Linq;
using PropertyInspection.Core.Entities;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Lookup mappings
            CreateMap<CountryLookup, CountryLookupDto>();
            CreateMap<CountryLookup, CountryDto>();
            CreateMap<StateLookup, StateLookupDto>();
            CreateMap<TimeZoneLookup, TimeZoneLookupDto>();

            // Agency mappings
            CreateMap<Agency, AgencyDto>();
            CreateMap<CreateAgencyDto, Agency>()
                .ForMember(d => d.CountryId, opt => opt.MapFrom(s => s.CountryId!.Value))
                .ForMember(d => d.StateId, opt => opt.MapFrom(s => s.StateId!.Value))
                .ForMember(d => d.TimeZoneId, opt => opt.MapFrom(s => s.TimeZoneId!.Value));
            CreateMap<UpdateAgencyDto, Agency>()
                .ForMember(d => d.CountryId, opt => opt.MapFrom(s => s.CountryId!.Value))
                .ForMember(d => d.StateId, opt => opt.MapFrom(s => s.StateId!.Value))
                .ForMember(d => d.TimeZoneId, opt => opt.MapFrom(s => s.TimeZoneId!.Value));

            // Agency whitelabel
            CreateMap<AgencyWhitelabel, AgencyWhitelabelDto>();
            CreateMap<CreateAgencyWhitelabelDto, AgencyWhitelabel>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
            CreateMap<UpdateAgencyWhitelabelDto, AgencyWhitelabel>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
            CreateMap<AgencyWhitelabel, WhitelabelBrandingDto>();
            CreateMap<AgencyWhitelabel, WhitelabelReportSettingsDto>()
                .ForMember(d => d.LogoUrl, opt => opt.MapFrom(s => s.LogoUrl))
                .ForMember(d => d.AgencyNameColor, opt => opt.Ignore())
                .ForMember(d => d.AddressColor, opt => opt.Ignore())
                .ForMember(d => d.AccentColor, opt => opt.Ignore())
                .ForMember(d => d.AccentFontFamily, opt => opt.Ignore())
                .ForMember(d => d.PrimaryColor, opt => opt.Ignore())
                .ForMember(d => d.SecondaryColor, opt => opt.Ignore())
                .ForMember(d => d.FontFamily, opt => opt.Ignore());
            CreateMap<DefaultWhitelabelDto, WhitelabelBrandingDto>();
            CreateMap<DefaultWhitelabelDto, WhitelabelReportSettingsDto>();

            // Users
            CreateMap<UserRole, UserRoleDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.RoleId))
                .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role != null ? s.Role.Name : string.Empty));
            CreateMap<User, UserDto>()
                .ForMember(d => d.AgencyName, opt => opt.MapFrom(s => s.Agency != null ? s.Agency.LegalBusinessName : null))
                .ForMember(d => d.UserRoles, opt => opt.MapFrom(s => s.UserRoles));
            CreateMap<User, UserDtoTemo>();

            // Property layout
            CreateMap<PropertyLayout, PropertyLayoutDto>()
                .ForMember(d => d.LayoutArea, opt => opt.MapFrom(s => s.Areas));
            CreateMap<LayoutArea, LayoutAreaDto>()
                .ForMember(d => d.LayoutItem, opt => opt.MapFrom(s => s.Items));
            CreateMap<LayoutItem, LayoutItemDto>();
            CreateMap<PropertyLayoutDto, PropertyLayout>()
                .ForMember(d => d.Areas, opt => opt.MapFrom(s => s.LayoutArea));
            CreateMap<LayoutAreaDto, LayoutArea>()
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.LayoutItem));
            CreateMap<LayoutItemDto, LayoutItem>();

            CreateMap<CreatePropertyLayoutDto, PropertyLayout>()
                .ForMember(d => d.Areas, opt => opt.MapFrom(s => s.LayoutArea));
            CreateMap<CreateLayoutAreaDto, LayoutArea>()
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.LayoutItem));
            CreateMap<CreateLayoutItemDto, LayoutItem>();

            // Property
            CreateMap<Property, PropertyDto>()
                .ForMember(d => d.PropertyManagerName, opt => opt.MapFrom(s =>
                    s.PropertyManager == null
                        ? null
                        : string.Join(" ", new[] { s.PropertyManager.FirstName, s.PropertyManager.LastName }
                            .Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.PropertyLayout, opt => opt.MapFrom(s => s.PropertyLayout))
                .ForMember(d => d.PropertyManager, opt => opt.MapFrom(s => s.PropertyManager))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.State))
                .ForMember(d => d.Landlords, opt => opt.MapFrom(s => s.Landlords))
                .ForMember(d => d.Tenancies, opt => opt.MapFrom(s => s.Tenancies));
            CreateMap<PropertyDto, Property>()
                .ForMember(d => d.Agency, opt => opt.Ignore())
                .ForMember(d => d.PropertyManager, opt => opt.Ignore())
                .ForMember(d => d.State, opt => opt.Ignore())
                .ForMember(d => d.PropertyLayout, opt => opt.Ignore())
                .ForMember(d => d.Landlords, opt => opt.Ignore())
                .ForMember(d => d.Tenancies, opt => opt.Ignore());
            CreateMap<Landlord, LandlordDto>().ReverseMap();
            CreateMap<Tenancy, TenancyDto>().ReverseMap();
            CreateMap<Tenant, TenantDto>().ReverseMap();

            // Inspection
            CreateMap<Inspection, InspectionDto>()
                .ForMember(d => d.PropertyAddress, opt => opt.MapFrom(s =>
                    s.Property == null
                        ? string.Empty
                        : string.Join(" ", new[] { s.Property.Address1, s.Property.Address2 }
                            .Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.PropertySubhurb, opt => opt.MapFrom(s => s.Property != null ? s.Property.CityOrSuburb : string.Empty))
                .ForMember(d => d.InspectorName, opt => opt.MapFrom(s =>
                    s.Inspector == null
                        ? string.Empty
                        : string.Join(" ", new[] { s.Inspector.FirstName, s.Inspector.LastName }
                            .Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.Property, opt => opt.MapFrom(s => s.Property))
                .ForMember(d => d.Agency, opt => opt.MapFrom(s => s.Agency))
                .ForMember(d => d.Inspector, opt => opt.MapFrom(s => s.Inspector));
            CreateMap<CreateInspectionDto, Inspection>();
            CreateMap<InspectionDto, Inspection>()
                .ForMember(d => d.Property, opt => opt.Ignore())
                .ForMember(d => d.Agency, opt => opt.Ignore())
                .ForMember(d => d.Inspector, opt => opt.Ignore())
                .ForMember(d => d.LandlordSnapshots, opt => opt.Ignore())
                .ForMember(d => d.TenancySnapshots, opt => opt.Ignore());

            CreateMap<LandlordSnapshot, LandlordSnapshotDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.LandlordId));
            CreateMap<TenancySnapshot, TenancySnapshotDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.TenancyId));

            // Reports
            CreateMap<Report, ReportDto>();
            CreateMap<ReportArea, ReportAreaDto>();
            CreateMap<ReportItem, ReportItemDto>();
            CreateMap<ReportItemCondition, ReportItemConditionDto>();
            CreateMap<ReportItemComment, ReportItemCommentDto>();
            CreateMap<ReportMedia, ReportMediaDto>();
            CreateMap<ReportMediaComment, ReportMediaCommentDto>();

            // Notifications
            CreateMap<CreateNotificationDto, Notification>();
            CreateMap<NotificationRecipient, UserNotificationDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Notification != null ? s.Notification.Title : string.Empty))
                .ForMember(d => d.Message, opt => opt.MapFrom(s => s.Notification != null ? s.Notification.Message : string.Empty))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(s => s.Notification != null ? s.Notification.CreatedAt : DateTime.MinValue));
            CreateMap<Agency, NotificationAgencyUserDto>()
                .ForMember(d => d.AgencyId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.AgencyName, opt => opt.MapFrom(s => s.LegalBusinessName))
                .ForMember(d => d.Users, opt => opt.MapFrom(s => s.Users));
            CreateMap<User, AgencyUserDto>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email ?? string.Empty))
                .ForMember(d => d.Fullname, opt => opt.MapFrom(s =>
                    string.Join(" ", new[] { s.FirstName, s.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)))));

            // Search
            CreateMap<Property, SearchResultDto>()
                .ForMember(d => d.Type, opt => opt.MapFrom(_ => "Property"))
                .ForMember(d => d.Address1, opt => opt.MapFrom(s => s.Address1 ?? string.Empty))
                .ForMember(d => d.Address2, opt => opt.MapFrom(s => s.Address2 ?? string.Empty))
                .ForMember(d => d.Subhurb, opt => opt.MapFrom(s => s.CityOrSuburb ?? string.Empty))
                .ForMember(d => d.TenantName, opt => opt.MapFrom(s => s.Tenancies.FirstOrDefault(t => t.IsActive) != null
                    ? s.Tenancies.FirstOrDefault(t => t.IsActive)!.FullName ?? string.Empty
                    : string.Empty))
                .ForMember(d => d.LandlordName, opt => opt.MapFrom(s => s.Landlords.FirstOrDefault() != null
                    ? s.Landlords.FirstOrDefault()!.Name ?? string.Empty
                    : string.Empty))
                .ForMember(d => d.InspectionDate, opt => opt.Ignore())
                .ForMember(d => d.InspectionType, opt => opt.Ignore());
            CreateMap<Inspection, SearchResultDto>()
                .ForMember(d => d.Type, opt => opt.MapFrom(_ => "Inspection"))
                .ForMember(d => d.Address1, opt => opt.MapFrom(s => s.Property != null ? s.Property.Address1 ?? string.Empty : string.Empty))
                .ForMember(d => d.Address2, opt => opt.MapFrom(s => s.Property != null ? s.Property.Address2 ?? string.Empty : string.Empty))
                .ForMember(d => d.Subhurb, opt => opt.MapFrom(s => s.Property != null ? s.Property.CityOrSuburb ?? string.Empty : string.Empty))
                .ForMember(d => d.TenantName, opt => opt.MapFrom(s => s.Property != null && s.Property.Tenancies.FirstOrDefault(t => t.IsActive) != null
                    ? s.Property.Tenancies.FirstOrDefault(t => t.IsActive)!.FullName ?? string.Empty
                    : string.Empty))
                .ForMember(d => d.LandlordName, opt => opt.MapFrom(s => s.Property != null && s.Property.Landlords.FirstOrDefault() != null
                    ? s.Property.Landlords.FirstOrDefault()!.Name ?? string.Empty
                    : string.Empty))
                .ForMember(d => d.InspectionDate, opt => opt.MapFrom(s => s.InspectionDate))
                .ForMember(d => d.InspectionType, opt => opt.MapFrom(s => s.InspectionType));
            CreateMap<Property, SearchPropertyDto>()
                .ForMember(d => d.Address, opt => opt.MapFrom(s => s.Address1 ?? s.Address2 ?? string.Empty));

            // Analytics
            CreateMap<Inspection, RecentInspectionDto>()
                .ForMember(d => d.PropertyAddress, opt => opt.MapFrom(s => s.Property != null ? s.Property.Address1 ?? string.Empty : string.Empty))
                .ForMember(d => d.InspectorName, opt => opt.MapFrom(s =>
                    s.Inspector == null
                        ? string.Empty
                        : string.Join(" ", new[] { s.Inspector.FirstName, s.Inspector.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.Date, opt => opt.MapFrom(s => s.InspectionDate))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.InspectionStatus.ToString()));
            CreateMap<Inspection, UpcomingInspectionDto>()
                .ForMember(d => d.PropertyAddress, opt => opt.MapFrom(s => s.Property != null ? s.Property.Address1 ?? string.Empty : string.Empty))
                .ForMember(d => d.InspectorName, opt => opt.MapFrom(s =>
                    s.Inspector == null
                        ? string.Empty
                        : string.Join(" ", new[] { s.Inspector.FirstName, s.Inspector.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.ScheduledDateTime, opt => opt.MapFrom(s => DateTime.SpecifyKind(s.InspectionDate, DateTimeKind.Utc)));
        }
    }
}

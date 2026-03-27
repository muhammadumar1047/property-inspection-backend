using AutoMapper;
using System;
using System.Linq;
using PropertyInspection.Core.Entities;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;
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
            CreateMap<Agency, AgencyResponse>()
                .ForMember(d => d.BillingPlanName, opt => opt.MapFrom(s => s.BillingPlan != null ? s.BillingPlan.Name : null));
            CreateMap<CreateAgencyRequest, Agency>()
                .ForMember(d => d.CountryId, opt => opt.MapFrom(s => s.CountryId!.Value))
                .ForMember(d => d.StateId, opt => opt.MapFrom(s => s.StateId!.Value))
                .ForMember(d => d.TimeZoneId, opt => opt.MapFrom(s => s.TimeZoneId!.Value));
            CreateMap<UpdateAgencyRequest, Agency>()
                .ForMember(d => d.CountryId, opt => opt.MapFrom(s => s.CountryId!.Value))
                .ForMember(d => d.StateId, opt => opt.MapFrom(s => s.StateId!.Value))
                .ForMember(d => d.TimeZoneId, opt => opt.MapFrom(s => s.TimeZoneId!.Value));

            // Agency whitelabel
            CreateMap<AgencyWhitelabel, AgencyWhitelabelResponse>();
            CreateMap<CreateAgencyWhitelabelRequest, AgencyWhitelabel>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
            CreateMap<UpdateAgencyWhitelabelRequest, AgencyWhitelabel>()
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
            CreateMap<Permission, PermissionDto>();

            CreateMap<UserRole, UserRoleDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.RoleId))
                .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role != null ? s.Role.Name : string.Empty))
                .ForMember(d => d.Permissions, opt => opt.MapFrom(s =>
                    s.Role != null
                        ? s.Role.RolePermissions.Select(rp => rp.Permission)
                        : Enumerable.Empty<Permission>()));

            CreateMap<User, UserResponse>()
                .ForMember(d => d.AgencyName, opt => opt.MapFrom(s => s.Agency != null ? s.Agency.LegalBusinessName : null))
                .ForMember(d => d.UserRoles, opt => opt.MapFrom(s => s.UserRoles));

            // Roles
            CreateMap<Role, RoleDto>();

            // Property layout
            CreateMap<PropertyLayout, PropertyLayoutResponse>()
                .ForMember(d => d.LayoutArea, opt => opt.MapFrom(s => s.Areas));
            CreateMap<LayoutArea, LayoutAreaResponse>()
                .ForMember(d => d.LayoutItem, opt => opt.MapFrom(s => s.Items));
            CreateMap<LayoutItem, LayoutItemResponse>();

            CreateMap<CreatePropertyLayoutRequest, PropertyLayout>()
                .ForMember(d => d.Areas, opt => opt.MapFrom(s => s.LayoutArea));
            CreateMap<UpdatePropertyLayoutRequest, PropertyLayout>()
                .ForMember(d => d.Areas, opt => opt.MapFrom(s => s.LayoutArea));

            CreateMap<CreateLayoutAreaRequest, LayoutArea>()
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.LayoutItem));
            CreateMap<CreateLayoutItemRequest, LayoutItem>();

            // Update layout mappings (nested collections)
            CreateMap<UpdateLayoutAreaRequest, LayoutArea>()
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.LayoutItem));
            CreateMap<UpdateLayoutItemRequest, LayoutItem>();

            // Property
            CreateMap<Property, PropertyResponse>()
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
            CreateMap<CreatePropertyRequest, Property>()
                .ForMember(d => d.Agency, opt => opt.Ignore())
                .ForMember(d => d.PropertyManager, opt => opt.Ignore())
                .ForMember(d => d.State, opt => opt.Ignore())
                .ForMember(d => d.PropertyLayout, opt => opt.Ignore())
                .ForMember(d => d.Landlords, opt => opt.Ignore())
                .ForMember(d => d.Tenancies, opt => opt.Ignore());
            CreateMap<UpdatePropertyRequest, Property>()
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
            CreateMap<Inspection, InspectionResponse>()
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
            CreateMap<CreateInspectionRequest, Inspection>();
            CreateMap<UpdateInspectionRequest, Inspection>()
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




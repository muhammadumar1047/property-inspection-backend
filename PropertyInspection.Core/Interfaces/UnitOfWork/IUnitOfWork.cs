using Microsoft.EntityFrameworkCore.Storage;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.Repositories;

namespace PropertyInspection.Core.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> Users { get; }
        IReportSyncRepository ReportSync { get; }
        IGenericRepository<Agency> Agencies { get; }
        IGenericRepository<AgencyWhitelabel> AgencyWhitelabels { get; }
        IGenericRepository<CountryLookup> Countries { get; }
        IGenericRepository<StateLookup> States { get; }
        IGenericRepository<TimeZoneLookup> TimeZones { get; }
        IGenericRepository<Property> Properties { get; }
        IGenericRepository<PropertyLayout> PropertyLayout { get; }
        IGenericRepository<LayoutArea> LayoutAreas { get; }
        IGenericRepository<LayoutItem> LayoutItems { get; }
        IGenericRepository<Inspection> Inspections { get; }
        IGenericRepository<Landlord> Landlords { get; }
        IGenericRepository<LandlordSnapshot> LandlordSnapshots { get; }
        IGenericRepository<Tenancy> Tenancies { get; }
        IGenericRepository<TenancySnapshot> TenancySnapshots { get; }
        IGenericRepository<Tenant> Tenants { get; }
        IGenericRepository<Report> Reports { get; }
        IGenericRepository<ReportArea> ReportAreas { get; }
        IGenericRepository<ReportItem> ReportItems { get; }
        IGenericRepository<ReportItemComment> ReportItemComments { get; }
        IGenericRepository<ReportItemCondition> ReportItemConditions { get; }
        IGenericRepository<ReportMedia> ReportMedias { get; }
        IGenericRepository<ReportMediaComment> ReportMediaComments { get; }
        IGenericRepository<Role> Roles { get; }
        IGenericRepository<Permission> Permissions { get; }
        IGenericRepository<UserRole> UserRoles { get; }
        IGenericRepository<RolePermission> RolePermissions { get; }

        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<NotificationRecipient> NotificationRecipients { get; }
        IGenericRepository<Billing> Billings { get; }

        Task<int> CommitAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}

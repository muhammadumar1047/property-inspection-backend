using Microsoft.EntityFrameworkCore.Storage;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.Repositories;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Infrastructure.Data;
using PropertyInspection.Infrastructure.Repositories;

namespace PropertyInspection.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private IGenericRepository<User>? _users;
        private IReportSyncRepository? _reportSync;
        private IGenericRepository<Agency>? _agencies;
        private IGenericRepository<AgencyWhitelabel>? _agencyWhitelabels;
        private IGenericRepository<CountryLookup>? _countries;
        private IGenericRepository<StateLookup>? _states;
        private IGenericRepository<TimeZoneLookup>? _timeZones;
        private IGenericRepository<Property>? _properties;
        private IGenericRepository<PropertyLayout>? _propertyLayout;
        private IGenericRepository<LayoutArea>? _layoutAreas;
        private IGenericRepository<LayoutItem>? _layoutItems;
        private IGenericRepository<Inspection>? _inspections;
        private IGenericRepository<Landlord>? _landlords;
        private IGenericRepository<LandlordSnapshot>? _landlordSnapshots;
        private IGenericRepository<Tenancy>? _tenancies;
        private IGenericRepository<TenancySnapshot>? _tenancySnapshots;
        private IGenericRepository<Tenant>? _tenants;
        private IGenericRepository<Report>? _reports;
        private IGenericRepository<ReportArea>? _reportAreas;
        private IGenericRepository<ReportItem>? _reportItems;
        private IGenericRepository<ReportItemComment>? _reportItemComments;
        private IGenericRepository<ReportItemCondition>? _reportItemConditions;
        private IGenericRepository<ReportMedia>? _reportMedias;
        private IGenericRepository<ReportMediaComment>? _reportMediaComments;
        private IGenericRepository<Role>? _roles;
        private IGenericRepository<Permission>? _permissions;
        private IGenericRepository<UserRole>? _userRoles;
        private IGenericRepository<RolePermission>? _rolePermissions;
        private IGenericRepository<Notification>? _notification;
        private IGenericRepository<NotificationRecipient>? _notificationRecipient;
        private IGenericRepository<Billing>? _billings;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IReportSyncRepository ReportSync => _reportSync ??= new ReportSyncRepository(_context);
        public IGenericRepository<User> Users => _users ??= new GenericRepository<User>(_context);
        public IGenericRepository<Agency> Agencies => _agencies ??= new GenericRepository<Agency>(_context);
        public IGenericRepository<AgencyWhitelabel> AgencyWhitelabels => _agencyWhitelabels ??= new GenericRepository<AgencyWhitelabel>(_context);
        public IGenericRepository<CountryLookup> Countries => _countries ??= new GenericRepository<CountryLookup>(_context);
        public IGenericRepository<StateLookup> States => _states ??= new GenericRepository<StateLookup>(_context);
        public IGenericRepository<TimeZoneLookup> TimeZones => _timeZones ??= new GenericRepository<TimeZoneLookup>(_context);
        public IGenericRepository<Property> Properties => _properties ??= new GenericRepository<Property>(_context);
        public IGenericRepository<PropertyLayout> PropertyLayout => _propertyLayout ??= new GenericRepository<PropertyLayout>(_context);
        public IGenericRepository<LayoutArea> LayoutAreas => _layoutAreas ??= new GenericRepository<LayoutArea>(_context);
        public IGenericRepository<LayoutItem> LayoutItems => _layoutItems ??= new GenericRepository<LayoutItem>(_context);
        public IGenericRepository<Inspection> Inspections => _inspections ??= new GenericRepository<Inspection>(_context);
        public IGenericRepository<Landlord> Landlords => _landlords ??= new GenericRepository<Landlord>(_context);
        public IGenericRepository<LandlordSnapshot> LandlordSnapshots => _landlordSnapshots ??= new GenericRepository<LandlordSnapshot>(_context);
        public IGenericRepository<Tenancy> Tenancies => _tenancies ??= new GenericRepository<Tenancy>(_context);
        public IGenericRepository<TenancySnapshot> TenancySnapshots => _tenancySnapshots ??= new GenericRepository<TenancySnapshot>(_context);
        public IGenericRepository<Tenant> Tenants => _tenants ??= new GenericRepository<Tenant>(_context);
        public IGenericRepository<Report> Reports => _reports ??= new GenericRepository<Report>(_context);
        public IGenericRepository<ReportArea> ReportAreas => _reportAreas ??= new GenericRepository<ReportArea>(_context);
        public IGenericRepository<ReportItem> ReportItems => _reportItems ??= new GenericRepository<ReportItem>(_context);
        public IGenericRepository<ReportItemComment> ReportItemComments => _reportItemComments ??= new GenericRepository<ReportItemComment>(_context);
        public IGenericRepository<ReportItemCondition> ReportItemConditions => _reportItemConditions ??= new GenericRepository<ReportItemCondition>(_context);
        public IGenericRepository<ReportMedia> ReportMedias => _reportMedias ??= new GenericRepository<ReportMedia>(_context);
        public IGenericRepository<ReportMediaComment> ReportMediaComments => _reportMediaComments ??= new GenericRepository<ReportMediaComment>(_context);
        public IGenericRepository<Role> Roles => _roles ??= new GenericRepository<Role>(_context);
        public IGenericRepository<Permission> Permissions => _permissions ??= new GenericRepository<Permission>(_context);
        public IGenericRepository<UserRole> UserRoles => _userRoles ??= new GenericRepository<UserRole>(_context);
        public IGenericRepository<RolePermission> RolePermissions => _rolePermissions ??= new GenericRepository<RolePermission>(_context);
        public IGenericRepository<Notification> Notifications => _notification ??= new GenericRepository<Notification>(_context);
        public IGenericRepository<NotificationRecipient> NotificationRecipients => _notificationRecipient ??= new GenericRepository<NotificationRecipient>(_context);
        public IGenericRepository<Billing> Billings => _billings ??= new GenericRepository<Billing>(_context);


        public async Task<int> CommitAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }

            try
            {
                var result = await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                return result;
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

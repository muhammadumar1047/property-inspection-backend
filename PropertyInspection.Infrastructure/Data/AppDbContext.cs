using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Core.Entities;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Infrastructure.Data.Seed;

namespace PropertyInspection.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        #region Users & Roles
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        #endregion

        #region Agencies
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<AgencyWhitelabel> AgencyWhitelabels { get; set; }
        #endregion

        #region Properties & Layouts
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyLayout> PropertyLayout { get; set; }
        public DbSet<LayoutArea> LayoutAreas { get; set; }
        public DbSet<LayoutItem> LayoutItems { get; set; }
        #endregion

        #region Inspections, Reports & Report Media

        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportArea> ReportAreas { get; set; }
        public DbSet<ReportItem> ReportItems { get; set; }
        public DbSet<ReportItemComment> ReportItemComments { get; set; }
        public DbSet<ReportItemCondition> ReportItemConditions { get; set; }
        public DbSet<ReportMedia> ReportMedias { get; set; }
        public DbSet<ReportMediaComment> ReportMediaComments { get; set; }
        #endregion

        #region Tenancies & Landlords
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<LandlordSnapshot> LandlordSnapshots { get; set; }
        public DbSet<Tenancy> Tenancies { get; set; }
        public DbSet<TenancySnapshot> TenancySnapshots { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        #endregion

        #region Lookups
        public DbSet<CountryLookup> CountryLookups { get; set; }
        public DbSet<StateLookup> StateLookups { get; set; }
        public DbSet<TimeZoneLookup> TimeZoneLookups { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<BillingFeature> BillingFeatures { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            builder.Entity<CountryLookup>().HasData(LookupSeedData.CountrySeedData);
            builder.Entity<StateLookup>().HasData(LookupSeedData.StateSeedData);
            builder.Entity<TimeZoneLookup>().HasData(LookupSeedData.TimezoneSeedData);

            #region Country , State & Timezone Configurations

            // Country relationship
            builder.Entity<Agency>()
                .HasOne(a => a.Country)
                .WithMany(c => c.Agencies)
                .HasForeignKey(a => a.CountryId) 
                .OnDelete(DeleteBehavior.Restrict);

            // State relationship
            builder.Entity<Agency>()
                .HasOne(a => a.State)
                .WithMany(s => s.Agencies)
                .HasForeignKey(a => a.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            // TimeZone relationship
            builder.Entity<Agency>()
                .HasOne(a => a.TimeZone)
                .WithMany(t => t.Agencies)
                .HasForeignKey(a => a.TimeZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            // Billing Plan relationship
            builder.Entity<Agency>()
                .HasOne(a => a.BillingPlan)
                .WithMany()
                .HasForeignKey(a => a.BillingPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}



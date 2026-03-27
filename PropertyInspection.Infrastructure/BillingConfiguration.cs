using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInspection.Core.Entities;

namespace PropertyInspection.Infrastructure.Data.Configurations
{
    public class BillingConfiguration : IEntityTypeConfiguration<Billing>
    {
        public void Configure(EntityTypeBuilder<Billing> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.PriceMonthly)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.PriceYearly)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.UserLimits)
                .IsRequired();

            builder.Property(x => x.TrialDays)
                .IsRequired();

            builder.HasMany(x => x.Features)
                .WithOne(f => f.Billing)
                .HasForeignKey(f => f.BillingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.IsActive);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}

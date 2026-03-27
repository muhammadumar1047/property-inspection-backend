using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInspection.Core.Entities;

namespace PropertyInspection.Infrastructure.Data.Configurations
{
    public class BillingFeatureConfiguration : IEntityTypeConfiguration<BillingFeature>
    {
        public void Configure(EntityTypeBuilder<BillingFeature> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasIndex(x => x.BillingId);
        }
    }
}

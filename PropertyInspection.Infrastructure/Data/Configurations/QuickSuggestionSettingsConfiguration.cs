using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInspection.Core.Entities;

namespace PropertyInspection.Infrastructure.Data.Configurations
{
    public class QuickSuggestionSettingsConfiguration : IEntityTypeConfiguration<QuickSuggestionSettings>
    {
        public void Configure(EntityTypeBuilder<QuickSuggestionSettings> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.AgencyId)
                .IsUnique();

            builder.HasOne(x => x.Agency)
                .WithMany()
                .HasForeignKey(x => x.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

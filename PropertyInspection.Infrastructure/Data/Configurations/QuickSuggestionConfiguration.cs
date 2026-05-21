using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInspection.Core.Entities;

namespace PropertyInspection.Infrastructure.Data.Configurations
{
    public class QuickSuggestionConfiguration : IEntityTypeConfiguration<QuickSuggestion>
    {
        public void Configure(EntityTypeBuilder<QuickSuggestion> builder)
        {
            builder.HasKey(x => x.Id);

            // Multitenant index for fast retrieval of specific dictionary suggestions
            builder.HasIndex(x => new { x.AgencyId, x.Type, x.IsDeleted })
                .HasDatabaseName("IX_QuickSuggestions_AgencyId_Type_IsDeleted");

            // Enforce text uniqueness within dictionary + agency + active records
            builder.HasIndex(x => new { x.AgencyId, x.Type, x.Text, x.IsDeleted })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = FALSE")
                .HasDatabaseName("UQ_QuickSuggestions_Text");

            // Enforce optional shortcut uniqueness within dictionary + agency + active records (ignoring nulls/empty)
            builder.HasIndex(x => new { x.AgencyId, x.Type, x.Shortcut, x.IsDeleted })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = FALSE AND \"Shortcut\" IS NOT NULL AND \"Shortcut\" != ''")
                .HasDatabaseName("UQ_QuickSuggestions_Shortcut");

            builder.HasOne(x => x.Agency)
                .WithMany()
                .HasForeignKey(x => x.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

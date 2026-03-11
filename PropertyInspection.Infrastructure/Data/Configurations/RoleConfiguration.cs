using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInspection.Core.Entities;

namespace PropertyInspection.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne(x => x.Agency)
            .WithMany(x => x.Roles)
            .HasForeignKey(x => x.AgencyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.AgencyId, x.Name })
            .IsUnique();

        builder.HasIndex(x => x.IsDeleted);
    }
}

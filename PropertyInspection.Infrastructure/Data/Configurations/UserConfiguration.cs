using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInspection.Core.Entities;

namespace PropertyInspection.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(x => x.Agency)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.AgencyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.IdentityUserId)
            .IsUnique();

        builder.HasIndex(x => x.AgencyId);
        builder.HasIndex(x => x.IsSuperAdmin);
        builder.HasIndex(x => x.IsDeleted);
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyInspection.Core.Authorization;
using PropertyInspection.Core.Entities;

namespace PropertyInspection.Infrastructure.Data.Seeder;

public static class PermissionSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Permissions
            .IgnoreQueryFilters()
            .ToDictionaryAsync(x => x.Name.ToLowerInvariant(), cancellationToken);

        var now = DateTime.UtcNow;

        foreach (var moduleEntry in PermissionCatalog.PermissionsByModule)
        {
            var module = moduleEntry.Key;
            foreach (var permissionName in moduleEntry.Value)
            {
                var key = permissionName.ToLowerInvariant();
                var description = BuildDescription(module, permissionName);

                if (existing.TryGetValue(key, out var current))
                {
                    var changed = false;
                    if (!string.Equals(current.Module, module, StringComparison.OrdinalIgnoreCase))
                    {
                        current.Module = module;
                        changed = true;
                    }

                    if (!string.Equals(current.Description, description, StringComparison.Ordinal))
                    {
                        current.Description = description;
                        changed = true;
                    }

                    if (current.IsDeleted)
                    {
                        current.IsDeleted = false;
                        current.DeletedAt = null;
                        current.DeletedBy = null;
                        changed = true;
                    }

                    if (changed)
                    {
                        current.UpdatedAt = now;
                        current.UpdatedBy = Guid.Empty;
                    }

                    continue;
                }

                var permission = new Permission
                {
                    Name = permissionName,
                    Module = module,
                    Description = description,
                    CreatedAt = now,
                    CreatedBy = Guid.Empty,
                    IsActive = true,
                    IsDeleted = false
                };

                await dbContext.Permissions.AddAsync(permission, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string BuildDescription(string module, string permissionName)
    {
        var action = permissionName.Split('.').LastOrDefault() ?? permissionName;
        return $"{module} {action}";
    }
}

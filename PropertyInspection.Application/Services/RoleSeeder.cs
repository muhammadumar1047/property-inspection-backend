using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Authorization;
using PropertyInspection.Core.Entities;
using PropertyInspection.Infrastructure.Data;

namespace PropertyInspection.Application.Services;

public class RoleSeeder : IAgencyRoleProvisioningService
{
    private static readonly IReadOnlyDictionary<string, string> RoleDescriptions = new Dictionary<string, string>
    {
        [DefaultRoleNames.AgencyAdmin] = "Full access to all agency modules and settings.",
        [DefaultRoleNames.AgencyManager] = "Operational management of properties, inspections, and reports.",
        [DefaultRoleNames.PropertyManager] = "Manages property layouts and property records.",
        [DefaultRoleNames.Inspector] = "Executes inspections and can view reports."
    };

    private readonly AppDbContext _dbContext;

    public RoleSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsureDefaultRolesForAllAgenciesAsync(CancellationToken cancellationToken = default)
    {
        var agencyIds = await _dbContext.Agencies
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var agencyId in agencyIds)
        {
            await EnsureDefaultRolesAsync(agencyId, Guid.Empty, cancellationToken);
        }
    }

    public async Task EnsureDefaultRolesAsync(Guid agencyId, Guid? actorUserId = null, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var actingUserId = actorUserId ?? Guid.Empty;

        var permissions = await _dbContext.Permissions
            .Where(x => !x.IsDeleted)
            .Select(x => new { x.Name, x.Id })
            .ToListAsync(cancellationToken);

        var permissionsByName = permissions.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

        var roles = await _dbContext.Roles
            .Where(x => x.AgencyId == agencyId)
            .ToListAsync(cancellationToken);

        var currentRoles = roles.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        var created = false;

        foreach (var roleName in DefaultRoleNames.All)
        {
            if (currentRoles.ContainsKey(roleName))
            {
                continue;
            }

            var role = new Role
            {
                Name = roleName,
                Description = RoleDescriptions[roleName],
                AgencyId = agencyId,
                CreatedAt = now,
                CreatedBy = actingUserId,
                IsActive = true,
                IsDeleted = false
            };

            await _dbContext.Roles.AddAsync(role, cancellationToken);
            currentRoles[roleName] = role;
            created = true;
        }

        if (created)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var roleIds = currentRoles.Values.Select(x => x.Id).ToList();
        var existingLinks = await _dbContext.RolePermissions
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => new { x.RoleId, x.PermissionId })
            .ToListAsync(cancellationToken);

        var existingSet = existingLinks
            .Select(x => (x.RoleId, x.PermissionId))
            .ToHashSet();

        var newLinks = new List<RolePermission>();

        foreach (var mapping in PermissionCatalog.DefaultRolePermissions)
        {
            if (!currentRoles.TryGetValue(mapping.Key, out var role))
            {
                continue;
            }

            foreach (var permissionName in mapping.Value)
            {
                if (!permissionsByName.TryGetValue(permissionName, out var permissionId))
                {
                    continue;
                }

                if (existingSet.Contains((role.Id, permissionId)))
                {
                    continue;
                }

                newLinks.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }
        }

        if (newLinks.Count == 0)
        {
            return;
        }

        await _dbContext.RolePermissions.AddRangeAsync(newLinks, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

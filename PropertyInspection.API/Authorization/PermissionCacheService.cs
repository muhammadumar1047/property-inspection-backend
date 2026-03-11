using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PropertyInspection.Application.IServices;
using PropertyInspection.Infrastructure.Data;

namespace PropertyInspection.API.Authorization;

public class PermissionCacheService : IPermissionCacheService
{
    private static readonly TimeSpan AbsoluteExpiration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(10);

    private readonly IMemoryCache _cache;
    private readonly AppDbContext _dbContext;

    public PermissionCacheService(IMemoryCache cache, AppDbContext dbContext)
    {
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task<IReadOnlySet<string>> GetPermissionsAsync(Guid userId, bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(userId);

        if (!forceRefresh && _cache.TryGetValue(cacheKey, out HashSet<string>? cached) && cached is not null)
        {
            return cached;
        }

        var agencyId = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.AgencyId)
            .FirstOrDefaultAsync(cancellationToken);

        var permissions = await _dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Where(ur => agencyId == null || ur.Role.AgencyId == agencyId)
            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Name))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .ToListAsync(cancellationToken);

        var normalized = permissions
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _cache.Set(cacheKey, normalized, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = AbsoluteExpiration,
            SlidingExpiration = SlidingExpiration
        });

        return normalized;
    }

    public Task SetPermissionsAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(userId);

        var normalized = permissions
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _cache.Set(cacheKey, normalized, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = AbsoluteExpiration,
            SlidingExpiration = SlidingExpiration
        });

        return Task.CompletedTask;
    }

    public Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _cache.Remove(BuildCacheKey(userId));
        return Task.CompletedTask;
    }

    private static string BuildCacheKey(Guid userId) => $"permissions_user_{userId}";
}

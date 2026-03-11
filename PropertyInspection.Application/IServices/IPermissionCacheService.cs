namespace PropertyInspection.Application.IServices;

public interface IPermissionCacheService
{
    Task<IReadOnlySet<string>> GetPermissionsAsync(Guid userId, bool forceRefresh = false, CancellationToken cancellationToken = default);
    Task SetPermissionsAsync(Guid userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);
    Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default);
}

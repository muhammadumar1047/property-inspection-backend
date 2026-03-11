namespace PropertyInspection.Application.IServices;

public interface IAgencyRoleProvisioningService
{
    Task EnsureDefaultRolesAsync(Guid agencyId, Guid? actorUserId = null, CancellationToken cancellationToken = default);
    Task EnsureDefaultRolesForAllAgenciesAsync(CancellationToken cancellationToken = default);
}

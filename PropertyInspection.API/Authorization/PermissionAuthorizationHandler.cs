using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PropertyInspection.Application.IServices;

namespace PropertyInspection.API.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionCacheService _permissionCache;
    private readonly ITenantContext _tenantContext;

    public PermissionAuthorizationHandler(IPermissionCacheService permissionCache, ITenantContext tenantContext)
    {
        _permissionCache = permissionCache;
        _tenantContext = tenantContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var isSuperAdmin = bool.TryParse(context.User.FindFirst("IsSuperAdmin")?.Value, out var superAdminFlag) && superAdminFlag;
        if (isSuperAdmin || _tenantContext.IsSuperAdmin)
        {
            context.Succeed(requirement);
            return;
        }

        if (_tenantContext.AgencyId is null)
        {
            return;
        }

        var domainUserIdClaim = context.User.FindFirst("DomainUserId")?.Value;
        if (!Guid.TryParse(domainUserIdClaim, out var domainUserId))
        {
            return;
        }

        var permissions = await _permissionCache.GetPermissionsAsync(domainUserId);

        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}

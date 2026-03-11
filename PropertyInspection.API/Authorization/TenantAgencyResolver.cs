using PropertyInspection.Application.IServices;

namespace PropertyInspection.API.Authorization;

public class TenantAgencyResolver : ITenantAgencyResolver
{
    private readonly ITenantContext _tenantContext;

    public TenantAgencyResolver(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public Guid ResolveAgencyId(Guid? requestedAgencyId = null)
    {
        if (_tenantContext.IsSuperAdmin)
        {
            if (requestedAgencyId is null)
            {
                throw new UnauthorizedAccessException("AgencyId is required for SuperAdmin.");
            }

            return requestedAgencyId.Value;
        }

        if (_tenantContext.AgencyId is null)
        {
            throw new UnauthorizedAccessException("Agency context is missing for the authenticated user.");
        }

        return _tenantContext.AgencyId.Value;
    }
}

namespace PropertyInspection.Application.IServices;

public interface ITenantAgencyResolver
{
    Guid ResolveAgencyId(Guid? requestedAgencyId = null);
}

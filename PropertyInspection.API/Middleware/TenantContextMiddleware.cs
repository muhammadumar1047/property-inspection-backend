using PropertyInspection.API.Extensions;

namespace PropertyInspection.API.Middleware
{
    public class TenantContextMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context, TenantContext tenant)
        {

            if (context.User.Identity?.IsAuthenticated == true)
            {
                tenant.IsSuperAdmin = context.User.IsSuperAdmin();

                if (!tenant.IsSuperAdmin)
                {
                    var orgClaim = context.User.GetAgencyId();
                    if (string.IsNullOrWhiteSpace(orgClaim))
                        throw new Exception("OrganizationId missing in user claims");

                    tenant.AgencyId = Guid.Parse(orgClaim);
                }

                // IP and User-Agent
                tenant.IsAgencyAdmin = context.User.IsAgencyAdmin();
                tenant.IdentityUserId = context.User.GetIdentityUserId();
                tenant.DomainUserId = context.User.GetDomainUserId();
                tenant.Ip = context.Connection.RemoteIpAddress?.ToString();
                tenant.UserAgent = context.Request.Headers["User-Agent"].ToString();
            }

            await _next(context);
        }

    }
}

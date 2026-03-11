using System.Security.Claims;

namespace PropertyInspection.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetIdentityUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        public static string GetDomainUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("DomainUserId") ?? string.Empty;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("Email") ?? string.Empty;
        }

        public static string GetFullName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("FullName") ?? string.Empty;
        }

        public static bool IsSuperAdmin(this ClaimsPrincipal user)
        {
            var val = user.FindFirstValue("IsSuperAdmin");
            return bool.TryParse(val, out var result) && result;
        }

        public static bool IsAgencyAdmin(this ClaimsPrincipal user)
        {
            var val = user.FindFirstValue("IsAgencyAdmin");
            return bool.TryParse(val, out var result) && result;
        }

        public static string? GetAgencyId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("AgencyId");
        }

        public static bool HasRole(this ClaimsPrincipal user, string role)
        {
            return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role);
        }

        public static Guid ResolveOrganizationId(this ClaimsPrincipal user, string? dtoOrgId = null)
        {
            if (user.IsSuperAdmin())
            {
                if (string.IsNullOrWhiteSpace(dtoOrgId))
                    throw new Exception("AgencyId is required for SuperAdmin");
                return Guid.Parse(dtoOrgId);
            }

            var orgIdClaim = user.GetAgencyId();
            if (string.IsNullOrWhiteSpace(orgIdClaim))
                throw new Exception("OrganizationId missing in user claims");

            return Guid.Parse(orgIdClaim);
        }
    }
}

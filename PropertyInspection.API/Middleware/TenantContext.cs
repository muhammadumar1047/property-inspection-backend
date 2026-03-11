using PropertyInspection.Application.IServices;

namespace PropertyInspection.API.Middleware
{
    public class TenantContext : ITenantContext
    {
        public Guid? AgencyId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAgencyAdmin { get; set; }
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
        public string? IdentityUserId { get; set; }
        public string? DomainUserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface ITenantContext
    {
        Guid? AgencyId { get; }
        bool IsSuperAdmin { get; }
        bool IsAgencyAdmin { get; }
        string? Ip { get; }
        string? UserAgent { get; }
        string? IdentityUserId { get; }
        string? DomainUserId { get; }
    }
}

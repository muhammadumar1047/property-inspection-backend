using Microsoft.AspNetCore.Authorization;

namespace PropertyInspection.API.Authorization;

public class PermissionAttribute : AuthorizeAttribute
{
    public PermissionAttribute(string permission)
    {
        Permission = permission;
    }

    public string Permission
    {
        get => Policy?.Replace(PermissionPolicyConstants.Prefix, string.Empty, StringComparison.Ordinal) ?? string.Empty;
        set => Policy = $"{PermissionPolicyConstants.Prefix}{value}";
    }
}

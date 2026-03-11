namespace PropertyInspection.Core.Authorization;

public static class PermissionCatalog
{
    public static class Modules
    {
        public const string PropertyLayout = "PropertyLayout";
        public const string Property = "Property";
        public const string Inspection = "Inspection";
        public const string Report = "Report";
        public const string UserManagement = "UserManagement";
        public const string RoleManagement = "RoleManagement";
    }

    public static readonly IReadOnlyDictionary<string, string[]> PermissionsByModule =
        new Dictionary<string, string[]>
        {
            [Modules.PropertyLayout] = ["propertylayout.create", "propertylayout.update", "propertylayout.delete", "propertylayout.view"],
            [Modules.Property] = ["property.create", "property.update", "property.delete", "property.view"],
            [Modules.Inspection] = ["inspection.create", "inspection.update", "inspection.delete", "inspection.view"],
            [Modules.Report] = ["report.generate", "report.view", "report.download"],
            [Modules.UserManagement] = ["user.create", "user.update", "user.delete", "user.view"],
            [Modules.RoleManagement] = ["role.create", "role.update", "role.delete", "role.view"]
        };

    public static readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> DefaultRolePermissions =
        new Dictionary<string, IReadOnlyCollection<string>>
        {
            [DefaultRoleNames.AgencyAdmin] = GetAllPermissions(),
            [DefaultRoleNames.AgencyManager] = GetPermissionsForModules(Modules.Property, Modules.Inspection, Modules.Report),
            [DefaultRoleNames.PropertyManager] = GetPermissionsForModules(Modules.PropertyLayout, Modules.Property),
            [DefaultRoleNames.Inspector] =
            [
                ..PermissionsByModule[Modules.Inspection],
                "report.view"
            ]
        };

    public static IReadOnlyCollection<string> GetAllPermissions() =>
        PermissionsByModule.Values.SelectMany(x => x).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

    public static IReadOnlyCollection<string> GetPermissionsForModules(params string[] modules) =>
        modules
            .Where(module => PermissionsByModule.ContainsKey(module))
            .SelectMany(module => PermissionsByModule[module])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}

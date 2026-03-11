namespace PropertyInspection.Core.Authorization;

public static class DefaultRoleNames
{
    public const string AgencyAdmin = "Agency Admin";
    public const string AgencyManager = "Agency Manager";
    public const string PropertyManager = "Property Manager";
    public const string Inspector = "Inspector";

    public static readonly IReadOnlyList<string> All =
    [
        AgencyAdmin,
        AgencyManager,
        PropertyManager,
        Inspector
    ];
}

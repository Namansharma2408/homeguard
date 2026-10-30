using System.ComponentModel;

namespace NetForge.Server.Features.Properties;

public static class PropertyPermissions
{
    [Description("View properties")]
    public const string Read = "properties.read";

    [Description("Create properties")]
    public const string Create = "properties.create";

    [Description("Edit properties")]
    public const string Update = "properties.update";

    [Description("Delete properties")]
    public const string Delete = "properties.delete";
}
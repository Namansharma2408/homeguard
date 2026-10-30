using System.ComponentModel;

namespace NetForge.Server.Features.Incidents;

public static class IncidentPermissions
{
    [Description("View incidents")]
    public const string Read = "incidents.read";

    [Description("Create incidents")]
    public const string Create = "incidents.create";

    [Description("Update incidents")]
    public const string Update = "incidents.update";

    [Description("Delete incidents")]
    public const string Delete = "incidents.delete";
}
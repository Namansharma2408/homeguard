using System.ComponentModel;

namespace NetForge.Server.Features.RiskMonitoring;

public static class RiskMonitoringPermissions
{
    [Description("View risk monitoring data")]
    public const string Read = "risk.read";

    [Description("Manage sensors and telemetry")]
    public const string Manage = "risk.manage";

    [Description("Resolve risk events")]
    public const string Resolve = "risk.resolve";
}
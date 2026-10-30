namespace NetForge.Server.Features.RiskMonitoring;

public static class RiskMonitoringMappings
{
    public static SensorDto ToDto(this Sensor e) => new(e.Id, e.PropertyId, e.Name, e.MetricType, e.CreatedAt);
    public static TelemetryReadingDto ToDto(this TelemetryReading e) => new(e.Id, e.SensorId, e.PropertyId, e.MetricType, e.Value, e.RecordedAt);
    public static RiskEventDto ToDto(this RiskEvent e) => new(e.Id, e.PropertyId, e.Category, e.Severity, e.Description, e.ObservedValue, e.RecommendedAction, e.Status, e.CreatedAt);
}
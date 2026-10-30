namespace NetForge.Server.Features.RiskMonitoring;

public class Sensor
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public required string Name { get; set; }
    public required string MetricType { get; set; } // humidity, temperature, water_leak, smoke, hvac, electrical
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class TelemetryReading
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public int PropertyId { get; set; }
    public required string MetricType { get; set; }
    public decimal Value { get; set; }
    public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class RiskEvent
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public required string Category { get; set; }
    public required string Severity { get; set; } // Low, Medium, High, Critical
    public required string Description { get; set; }
    public required string ObservedValue { get; set; }
    public required string RecommendedAction { get; set; }
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

// DTOs
public record SensorDto(int Id, int PropertyId, string Name, string MetricType, DateTimeOffset CreatedAt);
public record TelemetryReadingDto(int Id, int SensorId, int PropertyId, string MetricType, decimal Value, DateTimeOffset RecordedAt);
public record RiskEventDto(int Id, int PropertyId, string Category, string Severity, string Description, string ObservedValue, string RecommendedAction, string Status, DateTimeOffset CreatedAt);

public record RegisterSensorRequest(int PropertyId, string Name, string MetricType);
public record IngestTelemetryRequest(int SensorId, int PropertyId, string MetricType, decimal Value, DateTimeOffset? RecordedAt);
public record GenerateSampleReadingsRequest(int? PropertyId);

public record RiskAssessment(int PropertyId, int HealthScore, List<RiskEvent> NewEvents);
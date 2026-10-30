using Microsoft.EntityFrameworkCore;
using NetForge.Server.Data;
using NetForge.Server.Platform.Authorization;
using NetForge.Server.Platform.Features;
using NetForge.Server.Platform.Filters;
using NetForge.Server.Features.Properties;
using NetForge.Server.Platform.Pagination;

namespace NetForge.Server.Features.RiskMonitoring;

public sealed class RiskMonitoringEndpoints : IFeatureEndpoints
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/risk")
            .WithTags("RiskMonitoring")
            .AddEndpointFilter<ValidationFilter>()
            .AddEndpointFilter<PerformanceFilter>();

        group.MapGet("/sensors", ListSensors).RequirePermission(RiskMonitoringPermissions.Read);
        group.MapPost("/sensors", RegisterSensor).RequirePermission(RiskMonitoringPermissions.Manage).AddEndpointFilter<TransactionFilter>();
        group.MapGet("/readings", ListReadings).RequirePermission(RiskMonitoringPermissions.Read);
        group.MapPost("/telemetry", Ingest).RequirePermission(RiskMonitoringPermissions.Manage).AddEndpointFilter<TransactionFilter>();
        group.MapGet("/events", ListEvents).RequirePermission(RiskMonitoringPermissions.Read);
        group.MapPut("/events/{id:int}/status", UpdateEventStatus).RequirePermission(RiskMonitoringPermissions.Resolve).AddEndpointFilter<TransactionFilter>();
        group.MapPost("/sample/generate", GenerateSample).RequirePermission(RiskMonitoringPermissions.Manage).AddEndpointFilter<TransactionFilter>();
    }

    private static QuerySpec<RiskEvent> EventQuerySpec() => new QuerySpec<RiskEvent>()
        .Allow("severity", e => e.Severity)
        .Allow("status", e => e.Status)
        .Allow("category", e => e.Category)
        .Allow("propertyId", e => e.PropertyId)
        .Allow("createdAt", e => e.CreatedAt)
        .DefaultSort("createdAt", descending: true);

    private static async Task<IResult> ListSensors(int? propertyId, AppDbContext db, CancellationToken ct)
    {
        var q = db.Set<Sensor>().AsNoTracking();
        if (propertyId.HasValue) q = q.Where(s => s.PropertyId == propertyId.Value);
        var items = await q.OrderBy(s => s.Id).ToListAsync(ct);
        return Results.Ok(items.Select(s => s.ToDto()));
    }

    private static async Task<IResult> RegisterSensor(RegisterSensorRequest req, AppDbContext db, CancellationToken ct)
    {
        var propertyExists = await db.Set<Property>().AnyAsync(p => p.Id == req.PropertyId, ct);
        // Property entity is in Properties feature; reference by name
        if (!propertyExists) return Results.BadRequest(new { error = "Property not found" });

        var sensor = new Sensor { PropertyId = req.PropertyId, Name = req.Name, MetricType = req.MetricType, CreatedAt = DateTimeOffset.UtcNow };
        db.Set<Sensor>().Add(sensor);
        await db.SaveChangesAsync(ct);
        return Results.Created($"/api/risk/sensors/{sensor.Id}", sensor.ToDto());
    }

    private static async Task<IResult> ListReadings(int? propertyId, int? sensorId, AppDbContext db, CancellationToken ct)
    {
        var q = db.Set<TelemetryReading>().AsNoTracking();
        if (propertyId.HasValue) q = q.Where(r => r.PropertyId == propertyId.Value);
        if (sensorId.HasValue) q = q.Where(r => r.SensorId == sensorId.Value);
        var items = await q.OrderByDescending(r => r.RecordedAt).Take(200).ToListAsync(ct);
        return Results.Ok(items.Select(r => r.ToDto()));
    }

    private static async Task<IResult> Ingest(IngestTelemetryRequest req, AppDbContext db, CancellationToken ct)
    {
        var sensor = await db.Set<Sensor>().FirstOrDefaultAsync(s => s.Id == req.SensorId && s.PropertyId == req.PropertyId, ct);
        if (sensor is null) return Results.BadRequest(new { error = "Sensor/property mismatch" });
        if (!string.Equals(sensor.MetricType, req.MetricType, StringComparison.OrdinalIgnoreCase))
            return Results.BadRequest(new { error = "Metric type mismatch" });

        var reading = new TelemetryReading
        {
            SensorId = req.SensorId,
            PropertyId = req.PropertyId,
            MetricType = req.MetricType,
            Value = req.Value,
            RecordedAt = req.RecordedAt ?? DateTimeOffset.UtcNow,
        };
        db.Set<TelemetryReading>().Add(reading);

        // events from this reading only
        var (_, newEvents) = RiskEngine.Evaluate(req.PropertyId, new[] { reading });
        foreach (var ev in newEvents) db.Set<RiskEvent>().Add(ev);

        // recompute health score from all readings
        var allReadings = await db.Set<TelemetryReading>().Where(r => r.PropertyId == req.PropertyId).ToListAsync(ct);
        allReadings.Add(reading);
        var (score, _) = RiskEngine.Evaluate(req.PropertyId, allReadings);
        var property = await db.Set<Property>().FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct);
        if (property is not null) property.HealthScore = score;

        await db.SaveChangesAsync(ct);
        return Results.Created($"/api/risk/telemetry/{reading.Id}", reading.ToDto());
    }

    private static async Task<IResult> ListEvents(PagedRequest request, int? propertyId, AppDbContext db, CancellationToken ct)
    {
        var q = db.Set<RiskEvent>().AsNoTracking();
        if (propertyId.HasValue) q = q.Where(e => e.PropertyId == propertyId.Value);
        var paged = await q.ToPagedResultAsync(request, EventQuerySpec(), e => e.ToDto(), ct);
        return Results.Ok(paged);
    }

    private static async Task<IResult> UpdateEventStatus(int id, UpdateRiskEventStatusRequest req, AppDbContext db, CancellationToken ct)
    {
        var ev = await db.Set<RiskEvent>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ev is null) return Results.NotFound();
        ev.Status = req.Status;
        await db.SaveChangesAsync(ct);
        return Results.Ok(ev.ToDto());
    }

    private static async Task<IResult> GenerateSample(AppDbContext db, GenerateSampleReadingsRequest req, CancellationToken ct)
    {
        var rng = new Random(7);
        var propertyIds = req.PropertyId.HasValue
            ? new List<int> { req.PropertyId.Value }
            : await db.Set<Property>().Select(p => p.Id).ToListAsync(ct);

        var metricTypes = new[] { "humidity", "temperature", "water_leak", "smoke", "hvac", "electrical" };
        var sensorNames = new Dictionary<string, string>
        {
            ["humidity"] = "Humidity Sensor",
            ["temperature"] = "Temperature Sensor",
            ["water_leak"] = "Water Leak Detector",
            ["smoke"] = "Smoke Detector",
            ["hvac"] = "HVAC Monitor",
            ["electrical"] = "Electrical Monitor",
        };

        foreach (var pid in propertyIds)
        {
            // ensure sensors
            var sensors = await db.Set<Sensor>().Where(s => s.PropertyId == pid).ToListAsync(ct);
            foreach (var mt in metricTypes)
            {
                if (!sensors.Any(s => s.MetricType == mt))
                {
                    var s = new Sensor { PropertyId = pid, Name = sensorNames[mt], MetricType = mt, CreatedAt = DateTimeOffset.UtcNow };
                    db.Set<Sensor>().Add(s);
                    await db.SaveChangesAsync(ct);
                    sensors.Add(s);
                }
            }

            // generate readings
            foreach (var mt in metricTypes)
            {
                var sensor = sensors.First(s => s.MetricType == mt);
                int count = rng.Next(2, 5);
                for (int i = 0; i < count; i++)
                {
                    decimal value = mt switch
                    {
                        "humidity" => rng.Next(40, 95),
                        "temperature" => rng.Next(-5, 42),
                        "water_leak" => rng.Next(0, 10) < 2 ? 1 : 0,
                        "smoke" => rng.Next(0, 10) < 1 ? 1 : 0,
                        "hvac" => rng.Next(40, 95),
                        "electrical" => rng.Next(0, 10) < 2 ? 1 : 0,
                        _ => 0,
                    };
                    var reading = new TelemetryReading
                    {
                        SensorId = sensor.Id,
                        PropertyId = pid,
                        MetricType = mt,
                        Value = value,
                        RecordedAt = DateTimeOffset.UtcNow.AddHours(-rng.Next(0, 72)),
                    };
                    db.Set<TelemetryReading>().Add(reading);

                    var (_, evs) = RiskEngine.Evaluate(pid, new[] { reading });
                    foreach (var ev in evs) db.Set<RiskEvent>().Add(ev);
                }
            }

            // recompute health score
            var allReadings = await db.Set<TelemetryReading>().Where(r => r.PropertyId == pid).ToListAsync(ct);
            var (score, _) = RiskEngine.Evaluate(pid, allReadings);
            var property = await db.Set<Property>().FirstOrDefaultAsync(p => p.Id == pid, ct);
            if (property is not null) property.HealthScore = score;
        }

        await db.SaveChangesAsync(ct);
        return Results.Ok(new { generated = true });
    }
}

public record UpdateRiskEventStatusRequest(string Status);
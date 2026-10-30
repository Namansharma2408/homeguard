using Microsoft.EntityFrameworkCore;
using NetForge.Server.Data;
using NetForge.Server.Features.Incidents;
using NetForge.Server.Features.Properties;
using NetForge.Server.Features.RiskMonitoring;

namespace NetForge.Server.Data.Seed;

public static class HomeGuardSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();

        if (await db.Set<Property>().AnyAsync()) return; // idempotent

        // Properties
        var properties = new List<Property>
        {
            new() { Name = "Maple Residence", PropertyType = "House", Address = "12 Oak Lane", City = "Austin", State = "TX", Country = "USA", PostalCode = "78701", YearBuilt = 2012, Rooms = 4, SquareFeet = 2400, HealthScore = 95, CreatedAt = DateTimeOffset.UtcNow.AddDays(-60) },
            new() { Name = "Riverside Condo", PropertyType = "Condo", Address = "45 River Rd", City = "Seattle", State = "WA", Country = "USA", PostalCode = "98101", YearBuilt = 2018, Rooms = 2, SquareFeet = 1100, HealthScore = 72, CreatedAt = DateTimeOffset.UtcNow.AddDays(-50) },
            new() { Name = "Oakwood Townhouse", PropertyType = "Townhouse", Address = "88 Oakwood Dr", City = "Denver", State = "CO", Country = "USA", PostalCode = "80202", YearBuilt = 2005, Rooms = 3, SquareFeet = 1800, HealthScore = 60, CreatedAt = DateTimeOffset.UtcNow.AddDays(-45) },
            new() { Name = "Harbor View Apartment", PropertyType = "Apartment", Address = "3 Harbor Pl", City = "Boston", State = "MA", Country = "USA", PostalCode = "02108", YearBuilt = 2020, Rooms = 2, SquareFeet = 950, HealthScore = 88, CreatedAt = DateTimeOffset.UtcNow.AddDays(-40) },
            new() { Name = "Cedar Cottage", PropertyType = "House", Address = "7 Cedar Ct", City = "Portland", State = "OR", Country = "USA", PostalCode = "97201", YearBuilt = 1998, Rooms = 3, SquareFeet = 1600, HealthScore = 55, CreatedAt = DateTimeOffset.UtcNow.AddDays(-35) },
            new() { Name = "Summit Villa", PropertyType = "House", Address = "100 Summit Ave", City = "Aspen", State = "CO", Country = "USA", PostalCode = "81611", YearBuilt = 2015, Rooms = 5, SquareFeet = 3200, HealthScore = 98, CreatedAt = DateTimeOffset.UtcNow.AddDays(-30) },
            new() { Name = "Lakeview Duplex", PropertyType = "Duplex", Address = "22 Lakeview Dr", City = "Chicago", State = "IL", Country = "USA", PostalCode = "60601", YearBuilt = 2010, Rooms = 4, SquareFeet = 2100, HealthScore = 48, CreatedAt = DateTimeOffset.UtcNow.AddDays(-25) },
            new() { Name = "Garden Studio", PropertyType = "Studio", Address = "5 Garden Ln", City = "San Diego", State = "CA", Country = "USA", PostalCode = "92101", YearBuilt = 2022, Rooms = 1, SquareFeet = 520, HealthScore = 90, CreatedAt = DateTimeOffset.UtcNow.AddDays(-20) },
        };
        db.Set<Property>().AddRange(properties);
        await db.SaveChangesAsync();

        // Sensors & readings & risk events via RiskEngine
        var rng = new Random(99);
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

        foreach (var prop in properties)
        {
            foreach (var mt in metricTypes)
            {
                var sensor = new Sensor { PropertyId = prop.Id, Name = sensorNames[mt], MetricType = mt, CreatedAt = DateTimeOffset.UtcNow };
                db.Set<Sensor>().Add(sensor);
                await db.SaveChangesAsync();

                int count = rng.Next(2, 4);
                for (int i = 0; i < count; i++)
                {
                    decimal value = mt switch
                    {
                        "humidity" => rng.Next(45, 92),
                        "temperature" => rng.Next(-2, 40),
                        "water_leak" => (rng.Next(0, 10) < 2) ? 1 : 0,
                        "smoke" => (rng.Next(0, 12) < 1) ? 1 : 0,
                        "hvac" => rng.Next(45, 95),
                        "electrical" => (rng.Next(0, 10) < 2) ? 1 : 0,
                        _ => 0,
                    };
                    var reading = new TelemetryReading
                    {
                        SensorId = sensor.Id,
                        PropertyId = prop.Id,
                        MetricType = mt,
                        Value = value,
                        RecordedAt = DateTimeOffset.UtcNow.AddHours(-rng.Next(1, 120)),
                    };
                    db.Set<TelemetryReading>().Add(reading);

                    var (_, evs) = RiskEngine.Evaluate(prop.Id, new[] { reading });
                    foreach (var ev in evs) db.Set<RiskEvent>().Add(ev);
                }
            }

            // recompute health score
            var allReadings = await db.Set<TelemetryReading>().Where(r => r.PropertyId == prop.Id).ToListAsync();
            var (score, _) = RiskEngine.Evaluate(prop.Id, allReadings);
            prop.HealthScore = score;
        }
        await db.SaveChangesAsync();

        // Incidents
        var incidents = new List<Incident>
        {
            new() { PropertyId = properties[1].Id, Title = "Elevated humidity in bathroom", Description = "Humidity sensor reported sustained high levels.", Category = "Humidity", Priority = "Medium", Status = "Investigating", ReportedDate = DateTimeOffset.UtcNow.AddDays(-5), CreatedAt = DateTimeOffset.UtcNow.AddDays(-5) },
            new() { PropertyId = properties[2].Id, Title = "Water leak near kitchen sink", Description = "Leak detector triggered.", Category = "Water Leak", Priority = "Critical", Status = "InProgress", ReportedDate = DateTimeOffset.UtcNow.AddDays(-3), CreatedAt = DateTimeOffset.UtcNow.AddDays(-3) },
            new() { PropertyId = properties[4].Id, Title = "HVAC maintenance required", Description = "HVAC health degraded below threshold.", Category = "HVAC", Priority = "Medium", Status = "Reported", ReportedDate = DateTimeOffset.UtcNow.AddDays(-2), CreatedAt = DateTimeOffset.UtcNow.AddDays(-2) },
            new() { PropertyId = properties[6].Id, Title = "Electrical anomaly in panel", Description = "Electrical monitor triggered.", Category = "Electrical", Priority = "High", Status = "Resolved", ResolutionNotes = "Electrician inspected and replaced faulty breaker.", ReportedDate = DateTimeOffset.UtcNow.AddDays(-10), CreatedAt = DateTimeOffset.UtcNow.AddDays(-10) },
            new() { PropertyId = properties[0].Id, Title = "Smoke detector false alarm", Description = "Smoke sensor triggered briefly.", Category = "Smoke", Priority = "High", Status = "Closed", ResolutionNotes = "Sensor recalibrated; no fire found.", ReportedDate = DateTimeOffset.UtcNow.AddDays(-15), CreatedAt = DateTimeOffset.UtcNow.AddDays(-15) },
            new() { PropertyId = properties[3].Id, Title = "Abnormal temperature in bedroom", Description = "Temperature outside safe range.", Category = "Temperature", Priority = "Low", Status = "Resolved", ResolutionNotes = "Adjusted thermostat and verified insulation.", ReportedDate = DateTimeOffset.UtcNow.AddDays(-7), CreatedAt = DateTimeOffset.UtcNow.AddDays(-7) },
        };
        db.Set<Incident>().AddRange(incidents);
        await db.SaveChangesAsync();
    }
}
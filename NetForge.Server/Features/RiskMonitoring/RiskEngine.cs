namespace NetForge.Server.Features.RiskMonitoring;

/// <summary>
/// Deterministic, LLM-free risk assessment. Thresholds are demonstration rules, not certified
/// safety standards. Returns the computed health score and any risk events that should be recorded.
/// </summary>
public static class RiskEngine
{
    public static (int HealthScore, List<RiskEvent> Events) Evaluate(int propertyId, IEnumerable<TelemetryReading> readings)
    {
        int score = 100;
        var events = new List<RiskEvent>();

        foreach (var r in readings)
        {
            var metric = r.MetricType.ToLowerInvariant();
            switch (metric)
            {
                case "humidity":
                    if (r.Value >= 85)
                    {
                        score -= 25;
                        events.Add(Make(propertyId, "Humidity", "High", $"Humidity reading of {r.Value}% exceeds high-risk threshold (>=85).",
                            $"{r.Value}%", "Inspect ventilation and moisture sources; consider dehumidification."));
                    }
                    else if (r.Value >= 75)
                    {
                        score -= 12;
                        events.Add(Make(propertyId, "Humidity", "Medium", $"Humidity reading of {r.Value}% is elevated (>=75).",
                            $"{r.Value}%", "Increase airflow and monitor for condensation."));
                    }
                    break;
                case "temperature":
                    if (r.Value < 5 || r.Value > 35)
                    {
                        score -= 10;
                        events.Add(Make(propertyId, "Temperature", "Medium", $"Abnormal temperature reading of {r.Value}°C detected.",
                            $"{r.Value}°C", "Check HVAC and insulation; verify sensor placement."));
                    }
                    break;
                case "water_leak":
                case "waterleak":
                case "water leak":
                    if (r.Value >= 1)
                    {
                        score -= 40;
                        events.Add(Make(propertyId, "Water Leak", "Critical", "Water leakage detected by sensor.",
                            $"{r.Value}", "Shut off affected water supply and inspect plumbing immediately."));
                    }
                    break;
                case "smoke":
                    if (r.Value >= 1)
                    {
                        score -= 50;
                        events.Add(Make(propertyId, "Smoke", "Critical", "Smoke alert triggered.",
                            $"{r.Value}", "Evacuate and contact emergency services; inspect for fire source."));
                    }
                    break;
                case "hvac":
                    if (r.Value < 60)
                    {
                        score -= 15;
                        events.Add(Make(propertyId, "HVAC", "Medium", $"HVAC health at {r.Value}% indicates degradation.",
                            $"{r.Value}%", "Schedule HVAC maintenance and replace filters."));
                    }
                    break;
                case "electrical":
                    if (r.Value >= 1)
                    {
                        score -= 20;
                        events.Add(Make(propertyId, "Electrical", "High", "Electrical anomaly indicator triggered.",
                            $"{r.Value}", "Inspect electrical panel and wiring; consult a licensed electrician."));
                    }
                    break;
            }
        }

        score = Math.Clamp(score, 0, 100);
        return (score, events);
    }

    private static RiskEvent Make(int propertyId, string category, string severity, string description, string observed, string action) =>
        new()
        {
            PropertyId = propertyId,
            Category = category,
            Severity = severity,
            Description = description,
            ObservedValue = observed,
            RecommendedAction = action,
            Status = "Open",
            CreatedAt = DateTimeOffset.UtcNow,
        };
}
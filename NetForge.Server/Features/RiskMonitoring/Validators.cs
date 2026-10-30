using FluentValidation;

namespace NetForge.Server.Features.RiskMonitoring;

internal sealed class RegisterSensorValidator : AbstractValidator<RegisterSensorRequest>
{
    public RegisterSensorValidator()
    {
        RuleFor(x => x.PropertyId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MetricType).NotEmpty().MaximumLength(50);
    }
}

internal sealed class IngestTelemetryValidator : AbstractValidator<IngestTelemetryRequest>
{
    public IngestTelemetryValidator()
    {
        RuleFor(x => x.SensorId).GreaterThan(0);
        RuleFor(x => x.PropertyId).GreaterThan(0);
        RuleFor(x => x.MetricType).NotEmpty().MaximumLength(50);
    }
}
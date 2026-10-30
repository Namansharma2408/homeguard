using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetForge.Server.Features.RiskMonitoring;

internal sealed class SensorConfig : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> builder)
    {
        builder.ToTable("Sensors");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.MetricType).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.PropertyId);
    }
}

internal sealed class TelemetryReadingConfig : IEntityTypeConfiguration<TelemetryReading>
{
    public void Configure(EntityTypeBuilder<TelemetryReading> builder)
    {
        builder.ToTable("TelemetryReadings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MetricType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Value).HasPrecision(12, 4);
        builder.HasIndex(x => new { x.PropertyId, x.RecordedAt });
        builder.HasIndex(x => x.SensorId);
    }
}

internal sealed class RiskEventConfig : IEntityTypeConfiguration<RiskEvent>
{
    public void Configure(EntityTypeBuilder<RiskEvent> builder)
    {
        builder.ToTable("RiskEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Severity).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.ObservedValue).HasMaxLength(200).IsRequired();
        builder.Property(x => x.RecommendedAction).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => new { x.PropertyId, x.CreatedAt });
    }
}
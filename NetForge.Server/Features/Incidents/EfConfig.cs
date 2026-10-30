using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetForge.Server.Features.Incidents;

internal sealed class IncidentConfig : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("Incidents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Priority).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
        builder.Property(x => x.ResolutionNotes).HasMaxLength(2000);
        builder.HasIndex(x => new { x.PropertyId, x.Status });
    }
}
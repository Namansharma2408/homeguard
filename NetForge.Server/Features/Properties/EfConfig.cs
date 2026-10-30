using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetForge.Server.Features.Properties;

internal sealed class PropertyConfig : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PropertyType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(300).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();
        builder.Property(x => x.State).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Country).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PostalCode).HasMaxLength(20);
        builder.Property(x => x.SquareFeet).HasPrecision(12, 2);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.City);
    }
}
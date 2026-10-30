namespace NetForge.Server.Features.Properties;

public class Property
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string PropertyType { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Country { get; set; }
    public string? PostalCode { get; set; }
    public int? YearBuilt { get; set; }
    public int? Rooms { get; set; }
    public decimal? SquareFeet { get; set; }
    public int HealthScore { get; set; } = 100;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public record PropertyDto(
    int Id,
    string Name,
    string PropertyType,
    string Address,
    string City,
    string State,
    string Country,
    string? PostalCode,
    int? YearBuilt,
    int? Rooms,
    decimal? SquareFeet,
    int HealthScore,
    DateTimeOffset CreatedAt);

public record CreatePropertyRequest(
    string Name,
    string PropertyType,
    string Address,
    string City,
    string State,
    string Country,
    string? PostalCode,
    int? YearBuilt,
    int? Rooms,
    decimal? SquareFeet);

public record UpdatePropertyRequest(
    string Name,
    string PropertyType,
    string Address,
    string City,
    string State,
    string Country,
    string? PostalCode,
    int? YearBuilt,
    int? Rooms,
    decimal? SquareFeet);
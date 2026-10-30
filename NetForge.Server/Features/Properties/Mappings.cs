namespace NetForge.Server.Features.Properties;

public static class PropertyMappings
{
    public static PropertyDto ToDto(this Property entity) =>
        new(entity.Id, entity.Name, entity.PropertyType, entity.Address, entity.City, entity.State, entity.Country,
            entity.PostalCode, entity.YearBuilt, entity.Rooms, entity.SquareFeet, entity.HealthScore, entity.CreatedAt);
}
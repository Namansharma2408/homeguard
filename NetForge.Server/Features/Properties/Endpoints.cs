using Microsoft.EntityFrameworkCore;
using NetForge.Server.Data;
using NetForge.Server.Platform.Authorization;
using NetForge.Server.Platform.Features;
using NetForge.Server.Platform.Filters;
using NetForge.Server.Platform.Pagination;

namespace NetForge.Server.Features.Properties;

public sealed class PropertyEndpoints : IFeatureEndpoints
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/properties")
            .WithTags("Properties")
            .AddEndpointFilter<ValidationFilter>()
            .AddEndpointFilter<PerformanceFilter>();

        group.MapGet("/", List).RequirePermission(PropertyPermissions.Read);
        group.MapGet("/{id:int}", Get).RequirePermission(PropertyPermissions.Read);
        group.MapPost("/", Create).RequirePermission(PropertyPermissions.Create).AddEndpointFilter<TransactionFilter>();
        group.MapPut("/{id:int}", Update).RequirePermission(PropertyPermissions.Update).AddEndpointFilter<TransactionFilter>();
        group.MapDelete("/{id:int}", Delete).RequirePermission(PropertyPermissions.Delete).AddEndpointFilter<TransactionFilter>();
    }

    private static QuerySpec<Property> QuerySpec() => new QuerySpec<Property>()
        .Allow("name", p => p.Name)
        .Allow("city", p => p.City)
        .Allow("propertyType", p => p.PropertyType)
        .Allow("healthScore", p => p.HealthScore)
        .Allow("createdAt", p => p.CreatedAt)
        .Searchable(p => p.Name)
        .Searchable(p => p.City)
        .Searchable(p => p.Address)
        .DefaultSort("createdAt", descending: true);

    private static async Task<IResult> List(PagedRequest request, AppDbContext db, CancellationToken ct)
    {
        var paged = await db.Set<Property>().AsNoTracking().ToPagedResultAsync(request, QuerySpec(), p => p.ToDto(), ct);
        return Results.Ok(paged);
    }

    private static async Task<IResult> Get(int id, AppDbContext db, CancellationToken ct) =>
        await db.Set<Property>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct) is { } property
            ? Results.Ok(property.ToDto())
            : Results.NotFound();

    private static async Task<IResult> Create(CreatePropertyRequest req, AppDbContext db, CancellationToken ct)
    {
        var property = new Property
        {
            Name = req.Name,
            PropertyType = req.PropertyType,
            Address = req.Address,
            City = req.City,
            State = req.State,
            Country = req.Country,
            PostalCode = req.PostalCode,
            YearBuilt = req.YearBuilt,
            Rooms = req.Rooms,
            SquareFeet = req.SquareFeet,
            HealthScore = 100,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        db.Set<Property>().Add(property);
        await db.SaveChangesAsync(ct);
        return Results.Created($"/api/properties/{property.Id}", property.ToDto());
    }

    private static async Task<IResult> Update(int id, UpdatePropertyRequest req, AppDbContext db, CancellationToken ct)
    {
        var property = await db.Set<Property>().FirstOrDefaultAsync(p => p.Id == id, ct);
        if (property is null) return Results.NotFound();

        property.Name = req.Name;
        property.PropertyType = req.PropertyType;
        property.Address = req.Address;
        property.City = req.City;
        property.State = req.State;
        property.Country = req.Country;
        property.PostalCode = req.PostalCode;
        property.YearBuilt = req.YearBuilt;
        property.Rooms = req.Rooms;
        property.SquareFeet = req.SquareFeet;
        await db.SaveChangesAsync(ct);
        return Results.Ok(property.ToDto());
    }

    private static async Task<IResult> Delete(int id, AppDbContext db, CancellationToken ct)
    {
        var property = await db.Set<Property>().FirstOrDefaultAsync(p => p.Id == id, ct);
        if (property is null) return Results.NotFound();

        db.Set<Property>().Remove(property);
        await db.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}
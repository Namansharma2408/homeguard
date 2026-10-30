using Microsoft.EntityFrameworkCore;
using NetForge.Server.Data;
using NetForge.Server.Features.Properties;
using NetForge.Server.Platform.Authorization;
using NetForge.Server.Platform.Features;
using NetForge.Server.Platform.Filters;
using NetForge.Server.Platform.Pagination;

namespace NetForge.Server.Features.Incidents;

public sealed class IncidentEndpoints : IFeatureEndpoints
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/incidents")
            .WithTags("Incidents")
            .AddEndpointFilter<ValidationFilter>()
            .AddEndpointFilter<PerformanceFilter>();

        group.MapGet("/", List).RequirePermission(IncidentPermissions.Read);
        group.MapGet("/{id:int}", Get).RequirePermission(IncidentPermissions.Read);
        group.MapPost("/", Create).RequirePermission(IncidentPermissions.Create).AddEndpointFilter<TransactionFilter>();
        group.MapPut("/{id:int}/status", UpdateStatus).RequirePermission(IncidentPermissions.Update).AddEndpointFilter<TransactionFilter>();
        group.MapDelete("/{id:int}", Delete).RequirePermission(IncidentPermissions.Delete).AddEndpointFilter<TransactionFilter>();
    }

    private static QuerySpec<Incident> QuerySpec() => new QuerySpec<Incident>()
        .Allow("status", i => i.Status)
        .Allow("priority", i => i.Priority)
        .Allow("category", i => i.Category)
        .Allow("propertyId", i => i.PropertyId)
        .Allow("reportedDate", i => i.ReportedDate)
        .DefaultSort("reportedDate", descending: true);

    private static async Task<IResult> List(PagedRequest request, int? propertyId, AppDbContext db, CancellationToken ct)
    {
        var q = db.Set<Incident>().AsNoTracking();
        if (propertyId.HasValue) q = q.Where(i => i.PropertyId == propertyId.Value);
        var paged = await q.ToPagedResultAsync(request, QuerySpec(), i => i.ToDto(), ct);
        return Results.Ok(paged);
    }

    private static async Task<IResult> Get(int id, AppDbContext db, CancellationToken ct) =>
        await db.Set<Incident>().AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, ct) is { } inc
            ? Results.Ok(inc.ToDto())
            : Results.NotFound();

    private static async Task<IResult> Create(CreateIncidentRequest req, AppDbContext db, CancellationToken ct)
    {
        var propertyExists = await db.Set<Property>().AnyAsync(p => p.Id == req.PropertyId, ct);
        if (!propertyExists) return Results.BadRequest(new { error = "Property not found" });

        var incident = new Incident
        {
            PropertyId = req.PropertyId,
            Title = req.Title,
            Description = req.Description,
            Category = req.Category,
            Priority = req.Priority,
            Status = string.IsNullOrWhiteSpace(req.Status) ? "Reported" : req.Status,
            ReportedDate = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        db.Set<Incident>().Add(incident);
        await db.SaveChangesAsync(ct);
        return Results.Created($"/api/incidents/{incident.Id}", incident.ToDto());
    }

    private static async Task<IResult> UpdateStatus(int id, UpdateIncidentStatusRequest req, AppDbContext db, CancellationToken ct)
    {
        var incident = await db.Set<Incident>().FirstOrDefaultAsync(i => i.Id == id, ct);
        if (incident is null) return Results.NotFound();
        incident.Status = req.Status;
        if (!string.IsNullOrWhiteSpace(req.ResolutionNotes)) incident.ResolutionNotes = req.ResolutionNotes;
        await db.SaveChangesAsync(ct);
        return Results.Ok(incident.ToDto());
    }

    private static async Task<IResult> Delete(int id, AppDbContext db, CancellationToken ct)
    {
        var incident = await db.Set<Incident>().FirstOrDefaultAsync(i => i.Id == id, ct);
        if (incident is null) return Results.NotFound();
        db.Set<Incident>().Remove(incident);
        await db.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}
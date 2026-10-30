namespace NetForge.Server.Features.Incidents;

public class Incident
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Category { get; set; }
    public required string Priority { get; set; } // Low, Medium, High, Critical
    public string Status { get; set; } = "Reported"; // Reported, Investigating, InProgress, Resolved, Closed
    public DateTimeOffset ReportedDate { get; set; } = DateTimeOffset.UtcNow;
    public string? ResolutionNotes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public record IncidentDto(
    int Id,
    int PropertyId,
    string Title,
    string? Description,
    string Category,
    string Priority,
    string Status,
    DateTimeOffset ReportedDate,
    string? ResolutionNotes,
    DateTimeOffset CreatedAt);

public record CreateIncidentRequest(
    int PropertyId,
    string Title,
    string? Description,
    string Category,
    string Priority,
    string? Status);

public record UpdateIncidentStatusRequest(string Status, string? ResolutionNotes);
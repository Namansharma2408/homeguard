namespace NetForge.Server.Features.Incidents;

public static class IncidentMappings
{
    public static IncidentDto ToDto(this Incident e) =>
        new(e.Id, e.PropertyId, e.Title, e.Description, e.Category, e.Priority, e.Status, e.ReportedDate, e.ResolutionNotes, e.CreatedAt);
}
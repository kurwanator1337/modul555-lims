namespace Modul555.Lims.Domain.Common;

public class AuditLogEntry
{
    public long Id { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string? Changes { get; set; }
}

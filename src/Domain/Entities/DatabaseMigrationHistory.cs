namespace MonitorGlass.Domain.Entities;

public class DatabaseMigrationHistory : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string? FromDatabase { get; set; }
    public string? ToDatabase { get; set; }
    public string? Status { get; set; } // e.g. "Success", "Failed"
    public string? Message { get; set; } // Error or summary log
}
namespace MonitorGlass.Domain.Entities;

public class SqlDatabase : BaseAuditableEntity
{
    public string? Name { get; set; }
    public double SizeMB { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset LastBackup { get; set; }
    public bool IsSystemDatabase { get; set; }
    public Guid SqlServerInstanceId { get; set; }
    public SqlServerInstance SqlServerInstance { get; set; } = null!;
}

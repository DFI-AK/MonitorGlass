namespace MonitorGlass.Domain.Entities;

public class SqlServerInstance : BaseAuditableEntity
{
    public Guid ServerId { get; set; }
    public WindowsServer Server { get; set; } = null!;
    public string? InstanceName { get; set; }
    public string? Version { get; set; }
    public string? ConnectionString { get; set; }
    public bool IsDefault { get; set; }
    public bool IsConnected { get; set; }
    public ICollection<SqlDatabase> Databases { get; set; } = [];
    public ICollection<SqlServerMetric> SqlServerMetrics { get; set; } = [];
}
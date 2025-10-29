namespace MonitorGlass.Domain.Entities;

public class SqlServerMetric : BaseEntity<Guid>, IAuditableEntity
{
    public Guid SqlServerInstanceId { get; set; }
    public SqlServerInstance SqlServerInstance { get; set; } = null!;
    public double CpuUsagePercent { get; set; }
    public double MemoryUsageMB { get; set; }
    public int ActiveConnections { get; set; }
    public double DiskIOPS { get; set; }
    public double TransactionRatePerSec { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
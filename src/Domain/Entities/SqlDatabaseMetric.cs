namespace MonitorGlass.Domain.Entities;

public class SqlDatabaseMetric : BaseEntity<Guid>, IAuditableEntity
{
    public Guid SqlDatabaseId { get; set; }
    public SqlDatabase SqlDatabase { get; set; } = null!;
    public double DataFileSizeMB { get; set; }
    public double LogFileSizeMB { get; set; }
    public long TransactionCount { get; set; }
    public long DeadlockCount { get; set; }
    public double IndexFragmentationPercent { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
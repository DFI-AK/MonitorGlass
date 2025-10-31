namespace MonitorGlass.Domain.Entities;

public class SystemHealth : BaseEntity<Guid>, IAuditableEntity
{
    public Guid SystemInfoId { get; set; }
    public WindowsServer SystemInfo { get; set; } = null!;
    public bool IsOnline { get; set; } = true;
    public long UptimeMinutes { get; set; }
    public string? HealthStatus { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
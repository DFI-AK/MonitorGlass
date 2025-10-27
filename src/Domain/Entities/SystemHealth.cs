namespace MonitorGlass.Domain.Entities;

public class SystemHealth : BaseAuditableEntity
{
    public Guid SystemInfoId { get; set; }
    public SystemInfo SystemInfo { get; set; } = null!;
    public bool IsOnline { get; set; } = true;
    public long UptimeMinutes { get; set; }
    public string? HealthStatus { get; set; }
    public string? ErrorMessage { get; set; }
}
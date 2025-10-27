namespace MonitorGlass.Domain.Entities;

public class SystemInfo : BaseAuditableEntity
{
    public string? MachineName { get; set; }
    public string? OSVersion { get; set; }
    public ICollection<SystemMetric> Metrics { get; set; } = [];
    public ICollection<SystemHealth> SystemHealths { get; set; } = [];
}
namespace MonitorGlass.Domain.Entities;

public class WindowsServer : BaseAuditableEntity
{
    public string? MachineName { get; set; }
    public string? OSVersion { get; set; }
    public ICollection<SqlServerInstance> SqlServers { get; set; } = [];
    public ICollection<WindowsMetric> Metrics { get; set; } = [];
    public ICollection<SystemHealth> SystemHealths { get; set; } = [];
}
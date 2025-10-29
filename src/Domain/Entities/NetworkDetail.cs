namespace MonitorGlass.Domain.Entities;

public class NetworkDetail : BaseEntity<Guid>, IAuditableEntity
{
    public Guid SystemMetricId { get; set; }
    public SystemMetric SystemMetric { get; set; } = null!;
    public string InterfaceName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MACAddress { get; set; } = string.Empty;
    public string IPv4Address { get; set; } = string.Empty;
    public string? IPv6Address { get; set; }
    public bool IsUp { get; set; }
    public long SpeedMbps { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
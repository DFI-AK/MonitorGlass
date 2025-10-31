namespace MonitorGlass.Domain.Entities;

public class WindowsMetric : BaseEntity<Guid>, IAuditableEntity
{
    public Guid SystemInfoId { get; set; }
    public WindowsServer SystemInfo { get; set; } = null!;
    public CpuDetail CpuDetail { get; set; } = null!;
    public MemoryDetail MemoryDetail { get; set; } = null!;
    public ICollection<DiskDetail> DiskDetails { get; set; } = [];
    public ICollection<NetworkDetail> NetworkDetails { get; set; } = [];
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}

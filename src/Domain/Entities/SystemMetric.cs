namespace MonitorGlass.Domain.Entities;

public class SystemMetric : BaseEntity<Guid>, IAuditableEntity
{
    public Guid SystemInfoId { get; set; }
    public SystemInfo SystemInfo { get; set; } = null!;
    public CpuDetail CpuDetail { get; set; } = null!;
    public MemoryDetail MemoryDetail { get; set; } = null!;
    public ICollection<DiskDetail> DiskDetails { get; set; } = [];
    public ICollection<NetworkDetail> NetworkDetails { get; set; } = [];
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}

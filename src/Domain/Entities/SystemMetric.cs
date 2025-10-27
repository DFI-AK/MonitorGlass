namespace MonitorGlass.Domain.Entities;

public class SystemMetric : BaseAuditableEntity
{
    public Guid SystemInfoId { get; set; }
    public SystemInfo SystemInfo { get; set; } = null!;
    public CpuDetail CpuDetail { get; set; } = null!;
    public MemoryDetail MemoryDetail { get; set; } = null!;
    public ICollection<DiskDetail> DiskDetails { get; set; } = [];
    public ICollection<NetworkDetail> NetworkDetails { get; set; } = [];
}

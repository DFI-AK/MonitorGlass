namespace MonitorGlass.Domain.Entities;

public class DiskDetail : BaseAuditableEntity
{
    public Guid SystemMetricId { get; set; }
    public SystemMetric SystemMetric { get; set; } = null!;
    public string? DriveLetter { get; set; }
    public double? DiskReadSpeedMBps { get; set; }
    public double? DiskWriteSpeedMBps { get; set; }
    public int? DiskIOPS { get; set; }
    public double? DiskFreeSpaceGB { get; set; }
    public double? DiskTotalSpaceGB { get; set; }
}
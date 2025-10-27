namespace MonitorGlass.Domain.Entities;

public class CpuDetail
{
    public int CpuCores { get; set; }
    public double CpuCoreUsage { get; set; }
    public int CpuProcessCount { get; set; }
    public int CpuThreadCount { get; set; }
}
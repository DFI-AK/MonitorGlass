using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Interfaces;

public interface IWindowsHub
{
    Task WindowsMetrics(WindowsMetricDto metric);
}

using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Interfaces;

public interface IWindowsMetricRepository
{
    Task<WindowsMetric?> CreateSystemMetricAsync(WindowsMetric systemMetric, CancellationToken cancellationToken = default);
    Task<bool> DeleteSystemMetricAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WindowsMetric>> GetAllSystemMetricsAsync(CancellationToken cancellationToken = default);
    Task<WindowsMetric?> GetSystemMetricByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

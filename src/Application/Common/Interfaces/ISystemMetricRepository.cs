using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Interfaces;

public interface ISystemMetricRepository
{
    Task<SystemMetric?> CreateSystemMetricAsync(SystemMetric systemMetric, CancellationToken cancellationToken = default);
    Task<bool> DeleteSystemMetricAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemMetric>> GetAllSystemMetricsAsync(CancellationToken cancellationToken = default);
    Task<SystemMetric?> GetSystemMetricByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

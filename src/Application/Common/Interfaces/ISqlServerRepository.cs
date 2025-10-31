using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Interfaces;

public interface ISqlServerRepository
{
    Task<SqlServerInstance?> GetInstanceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SqlServerInstance>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default);
    Task UpdateInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default);

    Task AddDatabaseAsync(SqlDatabase database, CancellationToken cancellationToken = default);

    // Metrics
    Task AddInstanceMetricsAsync(SqlServerMetric metric, CancellationToken cancellationToken = default);
    Task AddDatabaseMetricsAsync(SqlDatabaseMetric metric, CancellationToken cancellationToken = default);

    Task<bool> IsInstanceExistAsync(string instanceName, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid instanceId, CancellationToken cancellationToken = default);
}
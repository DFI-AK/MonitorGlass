using MonitorGlass.Application.Common.Interfaces;

namespace MonitorGlass.Infrastructure.Services;

internal sealed class SqlServerMonitoringService(ISqlServerRepository sqlServerRepository)
{
    private readonly ISqlServerRepository _repository = sqlServerRepository;

    public async Task CollectAndStoreMetricsAsync(Guid instanceId, CancellationToken cancellationToken = default)
    {
        var instance = await _repository.GetInstanceAsync(instanceId, cancellationToken);
        if (instance is null) throw new ApplicationException("SQL Server instance not found.");

        // Collect Instance metrics
        var instanceMetrics = await SqlMetricCollector.CollectInstanceMetricsAsync(instance.ConnectionString!);
        instanceMetrics.SqlServerInstanceId = instance.Id;

        await _repository.AddInstanceMetricsAsync(instanceMetrics, cancellationToken);

        // Collect Database metrics
        foreach (var db in instance.Databases)
        {
            var dbMetrics = await SqlMetricCollector.CollectDatabaseMetricsAsync(instance.ConnectionString!, db.Name!);
            dbMetrics.SqlDatabaseId = db.Id;
            await _repository.AddDatabaseMetricsAsync(dbMetrics, cancellationToken);
        }
    }
}
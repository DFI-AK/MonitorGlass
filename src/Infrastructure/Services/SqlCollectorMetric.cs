using Microsoft.Data.SqlClient;
using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Infrastructure.Services;

public static class SqlMetricCollector
{
    public static async Task<SqlServerMetric> CollectInstanceMetricsAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                (SELECT COUNT(*) FROM sys.dm_exec_sessions) AS ActiveConnections,
                (SELECT cntr_value FROM sys.dm_os_performance_counters 
                    WHERE counter_name = 'User Connections') AS UserConnections,
                (SELECT TOP 1 total_physical_memory_kb / 1024 FROM sys.dm_os_sys_memory) AS TotalMemoryMB
        ";

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return new SqlServerMetric
        {
            ActiveConnections = reader.GetInt32(0),
            CpuUsagePercent = 0, // placeholder (can use perf counters)
            MemoryUsageMB = reader.GetDouble(2),
            DiskIOPS = 0, // optional: can use dm_io_virtual_file_stats
            TransactionRatePerSec = 0,
        };
    }

    public static async Task<SqlDatabaseMetric> CollectDatabaseMetricsAsync(string connectionString, string dbName, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = @$"
            USE [{dbName}];
            SELECT 
                SUM(size) * 8 / 1024.0 AS DataFileSizeMB
            FROM sys.database_files
            WHERE type_desc = 'ROWS';
        ";

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return new SqlDatabaseMetric
        {
            DataFileSizeMB = reader.GetDouble(0),
            LogFileSizeMB = 0,
            TransactionCount = 0,
            DeadlockCount = 0,
            IndexFragmentationPercent = 0,
        };
    }
}
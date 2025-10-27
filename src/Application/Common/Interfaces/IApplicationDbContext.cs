using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<SystemInfo> SystemInformations { get; }

    DbSet<SystemMetric> SystemMetrics { get; }

    DbSet<SqlServerInstance> SqlServerInstances { get; }

    DbSet<SqlDatabase> SqlDatabases { get; }

    DbSet<SystemHealth> Healths { get; }

    DbSet<DiskDetail> DiskDetails { get; }

    DbSet<NetworkDetail> NetworkDetails { get; }

    DbSet<DatabaseMigrationHistory> DatabaseMigrationHistory { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

using System.Reflection;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MonitorGlass.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IApplicationDbContext
{
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<WindowsServer> Windows => Set<WindowsServer>();

    public DbSet<WindowsMetric> WindowsMetrics => Set<WindowsMetric>();

    public DbSet<SqlServerInstance> SqlServerInstances => Set<SqlServerInstance>();

    public DbSet<SqlDatabase> SqlDatabases => Set<SqlDatabase>();

    public DbSet<SystemHealth> Healths => Set<SystemHealth>();

    public DbSet<DiskDetail> DiskDetails => Set<DiskDetail>();

    public DbSet<NetworkDetail> NetworkDetails => Set<NetworkDetail>();

    public DbSet<DatabaseMigrationHistory> DatabaseMigrationHistory => Set<DatabaseMigrationHistory>();

    public DbSet<SqlServerMetric> SqlServerMetrics => Set<SqlServerMetric>();

    public DbSet<SqlDatabaseMetric> SqlDatabaseMetrics => Set<SqlDatabaseMetric>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

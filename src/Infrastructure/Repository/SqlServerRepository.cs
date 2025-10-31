using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Application.Common.Models;
using MonitorGlass.Domain.Entities;
using MonitorGlass.Infrastructure.Data;

namespace MonitorGlass.Infrastructure.Repository;

internal sealed class SqlServerRepository(ILogger<SqlServerRepository> logger, ApplicationDbContext context) : ISqlServerRepository
{
    private readonly ILogger<SqlServerRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task AddDatabaseAsync(SqlDatabase database, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding database to '{instanceId}'", database.SqlServerInstanceId);
        await _context.SqlDatabases.AddAsync(database, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Database is added successfully.");
    }

    public async Task AddDatabaseMetricsAsync(SqlDatabaseMetric metric, CancellationToken cancellationToken = default)
    {
        _context.SqlDatabaseMetrics.Add(metric);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new sql server instance...");

        if (string.IsNullOrEmpty(instance.InstanceName)) throw new InvalidOperationException("Instance name should not be empty or null");

        await _context.SqlServerInstances.AddAsync(instance, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Instance is added successfully.");
    }

    public async Task AddInstanceMetricsAsync(SqlServerMetric metric, CancellationToken cancellationToken = default)
    {
        _context.SqlServerMetrics.Add(metric);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> DeleteAsync(Guid instanceId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.SqlServerInstances.FirstOrDefaultAsync(x => x.Id == instanceId, cancellationToken)
        ?? throw new KeyNotFoundException($"Instance with id '{instanceId}' not found.");
        _context.SqlServerInstances.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<IEnumerable<SqlServerInstance>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = _context.SqlServerInstances
            .Include(x => x.Databases)
            .AsNoTracking();

        return await Task.Run(() => entities);
    }

    public async Task<SqlServerInstance?> GetInstanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.SqlServerInstances
            .Include(x => x.Databases)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity;
    }

    public async Task<bool> IsInstanceExistAsync(string instanceName, CancellationToken cancellationToken)
    {
        var instance = await _context.SqlServerInstances.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.InstanceName) && x.InstanceName.ToLower() == instanceName.ToLower(), cancellationToken: cancellationToken);
        return instance is not null;
    }

    public async Task UpdateInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updateing instance '{instanceName}'", instance.InstanceName);

        _context.SqlServerInstances.Update(instance);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Sql instance is updated successfully.");
    }
}
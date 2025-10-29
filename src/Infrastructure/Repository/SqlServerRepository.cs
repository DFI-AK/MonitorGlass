using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Entities;
using MonitorGlass.Infrastructure.Data;

namespace MonitorGlass.Infrastructure.Repository;

internal sealed class SqlServerRepository(ILogger<SqlServerRepository> logger, ApplicationDbContext context, EncryptionService encryption) : ISqlServerRepository
{
    private readonly ILogger<SqlServerRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;
    private readonly EncryptionService _encryption = encryption;

    public async Task AddDatabaseAsync(SqlDatabase database, CancellationToken cancellationToken = default)
    {
        database.Name = _encryption.EncryptData(database.Name ?? string.Empty);
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

        instance.InstanceName = _encryption.EncryptData(instance.InstanceName);

        instance.ConnectionString = _encryption.EncryptData(instance.ConnectionString ?? throw new ArgumentNullException(nameof(instance.ConnectionString), "Connection string cannot be null or empty."));

        await _context.SqlServerInstances.AddAsync(instance, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Instance is added successfully.");
    }

    public async Task AddInstanceMetricsAsync(SqlServerMetric metric, CancellationToken cancellationToken = default)
    {
        _context.SqlServerMetrics.Add(metric);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SqlServerInstance>> GetAllAsync(CancellationToken cancellationToken = default)
    => await _context.SqlServerInstances
            .Include(x => x.Databases)
            .ToListAsync(cancellationToken);

    public async Task<SqlServerInstance?> GetInstanceAsync(Guid id, CancellationToken cancellationToken = default)
     => await _context.SqlServerInstances
            .Include(x => x.Databases)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpdateInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updateing instance '{instanceName}'", instance.InstanceName);
        _context.SqlServerInstances.Update(instance);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Sql instance is updated successfully.");
    }
}
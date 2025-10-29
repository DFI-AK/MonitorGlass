using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Application.Common.Models;
using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Infrastructure.Services;

internal sealed class SqlServerInstanceService(ISqlServerRepository sqlServerRepository, ISqlConnectionValidatorService service)
{
    private readonly ISqlConnectionValidatorService _sqlConnectionValidator = service;
    private readonly ISqlServerRepository _repository = sqlServerRepository;
    public async Task<Result> AddInstanceAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var validation = await _sqlConnectionValidator.ValidateInstanceAsync(connectionString);
        if (!validation.IsValid)
            return Result.Failure([$"Connection failed: {validation.Error}"]);

        var instance = new SqlServerInstance
        {
            InstanceName = validation.InstanceName,
            Version = validation.Version,
            ConnectionString = connectionString,
            IsConnected = true,
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };

        await _repository.AddInstanceAsync(instance, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> AddDatabaseAsync(SqlDatabase database, CancellationToken cancellationToken = default)
    {
        var instance = await _repository.GetInstanceAsync(database.SqlServerInstanceId, cancellationToken);
        if (instance == null)
            return Result.Failure(["SQL Server instance not found."]);

        var validation = await _sqlConnectionValidator.ValidateDatabaseAsync(instance.ConnectionString!, database.Name ?? string.Empty);
        if (!validation.Exists)
            return Result.Failure([$"Database '{database.Name}' does not exist or cannot be accessed. {validation.Error}"]);

        await _repository.AddDatabaseAsync(database, cancellationToken);
        return Result.Success();
    }
}
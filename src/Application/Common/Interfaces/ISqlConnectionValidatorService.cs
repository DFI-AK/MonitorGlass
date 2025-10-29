namespace MonitorGlass.Application.Common.Interfaces;

public interface ISqlConnectionValidatorService
{
    Task<(bool IsValid, string? InstanceName, string? Version, string? Error)> ValidateInstanceAsync(string connectionString);
    Task<(bool Exists, string? Error)> ValidateDatabaseAsync(string connectionString, string databaseName);
}
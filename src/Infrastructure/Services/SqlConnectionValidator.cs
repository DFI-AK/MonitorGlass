using Microsoft.Data.SqlClient;
using MonitorGlass.Application.Common.Interfaces;

namespace MonitorGlass.Infrastructure.Services;

internal sealed class SqlConnectionValidatorService : ISqlConnectionValidatorService
{
    public async Task<(bool Exists, string? Error)> ValidateDatabaseAsync(string connectionString, string databaseName)
    {

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName;";
            command.Parameters.AddWithValue("@dbName", databaseName);

            var count = await command.ExecuteScalarAsync();
            if (count is null) throw new ApplicationException("Count is null");

            return ((int)count > 0, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool IsValid, string? InstanceName, string? Version, string? Error)> ValidateInstanceAsync(string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Query basic instance info
            var command = connection.CreateCommand();
            command.CommandText = "SELECT @@SERVERNAME AS InstanceName, @@VERSION AS Version;";

            using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            var instanceName = reader.GetString(0);
            var version = reader.GetString(1);

            return (true, instanceName, version, null);
        }
        catch (Exception ex)
        {
            return (false, null, null, ex.Message);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Entities;
using MonitorGlass.Infrastructure.Data;

namespace MonitorGlass.Infrastructure.Repository;

internal sealed class SystemInformationRepository(ILogger<SystemInformationRepository> logger, ApplicationDbContext context) : ISystemInformationRepository
{
    private readonly ILogger<SystemInformationRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task<SystemInfo?> CreateSystemInfoAsync(SystemInfo systemInfo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new SystemInfo with ID: {SystemInfoId}", systemInfo.Id);
        await _context.SystemInformations.AddAsync(systemInfo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully created SystemInfo with ID: {SystemInfoId}", systemInfo.Id);
        return systemInfo;
    }

    public async Task<bool> DeleteSystemInfoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var systemInfo = await _context.SystemInformations.FindAsync([id], cancellationToken);
        if (systemInfo == null)
        {
            _logger.LogWarning("SystemInfo with ID: {SystemInfoId} not found", id);
            return false;
        }

        _context.SystemInformations.Remove(systemInfo);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully deleted SystemInfo with ID: {SystemInfoId}", id);
        return true;
    }

    public async Task<IEnumerable<SystemInfo>> GetAllSystemInfosAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.SystemInformations.AsNoTracking().ToListAsync(cancellationToken);
        return entities;
    }

    public async Task<SystemInfo?> GetSystemInfoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var systemInfo = await _context.SystemInformations.FindAsync([id], cancellationToken);
        if (systemInfo == null)
        {
            _logger.LogWarning("SystemInfo with ID: {SystemInfoId} not found", id);
        }
        return systemInfo;
    }

    public async Task<bool> IsExistAsync(string machineName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(machineName)) throw new ArgumentNullException(nameof(machineName), "Machine name cannot be null or empty.");
        var entityExists = await _context.SystemInformations.AnyAsync(x => x.MachineName == machineName, cancellationToken);
        return entityExists;
    }

    public async Task<SystemInfo?> UpdateSystemInfoAsync(SystemInfo systemInfo, CancellationToken cancellationToken = default)
    {
        var existingSystemInfo = await _context.SystemInformations.FindAsync([systemInfo.Id], cancellationToken);
        if (existingSystemInfo == null)
        {
            _logger.LogWarning("SystemInfo with ID: {SystemInfoId} not found", systemInfo.Id);
            return null;
        }

        _context.Entry(existingSystemInfo).CurrentValues.SetValues(systemInfo);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully updated SystemInfo with ID: {SystemInfoId}", systemInfo.Id);
        return systemInfo;
    }
}

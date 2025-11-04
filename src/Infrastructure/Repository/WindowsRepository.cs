using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Entities;
using MonitorGlass.Infrastructure.Data;

namespace MonitorGlass.Infrastructure.Repository;

internal sealed class WindowsRepository(ILogger<WindowsRepository> logger, ApplicationDbContext context) : IWindowsRepository
{
    private readonly ILogger<WindowsRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task<WindowsServer?> CreateSystemInfoAsync(WindowsServer systemInfo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new SystemInfo with ID: {SystemInfoId}", systemInfo.Id);
        await _context.Windows.AddAsync(systemInfo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully created SystemInfo with ID: {SystemInfoId}", systemInfo.Id);
        return systemInfo;
    }

    public async Task<bool> DeleteSystemInfoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var systemInfo = await _context.Windows.FindAsync([id], cancellationToken);
        if (systemInfo == null)
        {
            _logger.LogWarning("SystemInfo with ID: {SystemInfoId} not found", id);
            return false;
        }

        _context.Windows.Remove(systemInfo);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully deleted SystemInfo with ID: {SystemInfoId}", id);
        return true;
    }

    public async Task<IEnumerable<WindowsServer>> GetAllSystemInfosAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Windows.AsNoTracking().ToListAsync(cancellationToken);
        return entities;
    }

    public async Task<WindowsServer?> GetSystemInfoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var systemInfo = await _context.Windows.FindAsync([id], cancellationToken);
        if (systemInfo == null)
        {
            _logger.LogWarning("SystemInfo with ID: {SystemInfoId} not found", id);
        }
        return systemInfo;
    }

    public async Task<WindowsServer?> GetSystemInfoByNameAsync(string serverName, CancellationToken cancellationToken = default)
    => await _context.Windows.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.MachineName) && (x.MachineName.ToLower() == serverName.ToLower()), cancellationToken);

    public async Task<WindowsServer?> GetWindowsServerAsync(CancellationToken cancellationToken = default)
     => await _context.Windows.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> IsExistAsync(string machineName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(machineName)) throw new ArgumentNullException(nameof(machineName), "Machine name cannot be null or empty.");
        var entityExists = await _context.Windows.AnyAsync(x => x.MachineName == machineName, cancellationToken);
        return entityExists;
    }

    public async Task<WindowsServer?> UpdateSystemInfoAsync(WindowsServer systemInfo, CancellationToken cancellationToken = default)
    {
        var existingSystemInfo = await _context.Windows.FindAsync([systemInfo.Id], cancellationToken);
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Entities;
using MonitorGlass.Infrastructure.Data;

namespace MonitorGlass.Infrastructure.Repository;

internal sealed class WindowsMetricRepository(ILogger<WindowsMetricRepository> logger, ApplicationDbContext context) : IWindowsMetricRepository
{
    private readonly ILogger<WindowsMetricRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task<WindowsMetric?> CreateSystemMetricAsync(WindowsMetric systemMetric, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new SystemMetric with ID: {SystemMetricId}", systemMetric.Id);
        await _context.WindowsMetrics.AddAsync(systemMetric, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully created SystemMetric with ID: {SystemMetricId}", systemMetric.Id);
        return systemMetric;
    }

    public async Task<bool> DeleteSystemMetricAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var systemMetric = await _context.WindowsMetrics.FindAsync([id], cancellationToken);
        if (systemMetric == null)
        {
            _logger.LogWarning("SystemMetric with ID: {SystemMetricId} not found", id);
            return false;
        }

        _context.WindowsMetrics.Remove(systemMetric);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully deleted SystemMetric with ID: {SystemMetricId}", id);
        return true;
    }

    public async Task<IEnumerable<WindowsMetric>> GetAllSystemMetricsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all SystemMetrics");
        var systemMetrics = await _context.WindowsMetrics.ToListAsync(cancellationToken);
        _logger.LogInformation("Successfully retrieved {SystemMetricsCount} SystemMetrics", systemMetrics.Count);
        return systemMetrics;
    }

    public async Task<WindowsMetric?> GetSystemMetricByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving SystemMetric with ID: {SystemMetricId}", id);
        var systemMetric = await _context.WindowsMetrics.FindAsync([id], cancellationToken);
        if (systemMetric == null)
        {
            _logger.LogWarning("SystemMetric with ID: {SystemMetricId} not found", id);
        }
        else
        {
            _logger.LogInformation("Successfully retrieved SystemMetric with ID: {SystemMetricId}", id);
        }
        return systemMetric;
    }
}

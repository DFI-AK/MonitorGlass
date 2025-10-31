using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Interfaces;

public interface IWindowsProbeService
{
    Task<bool> CheckServerAvailabilityAsync(string hostName, CancellationToken cancellationToken = default);
    Task<string> GetLocalOperatingSystemAsync(string hostName, CancellationToken cancellationToken = default);
    Task<string> GetOperatingSystemRemoteAsync(string hostName, string username, string password, CancellationToken cancellationToken = default);
    Task<WindowsMetric> CollectSystemMetricsAsync(string hostName, CancellationToken cancellationToken = default);
}

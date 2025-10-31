using Microsoft.Extensions.Hosting;

namespace MonitorGlass.Application.Common.Interfaces;

public interface IWindowsMetricWorker : IHostedService, IDisposable
{
    Task StartCollectMetricsAsync(CancellationToken cancellationToken = default);
}

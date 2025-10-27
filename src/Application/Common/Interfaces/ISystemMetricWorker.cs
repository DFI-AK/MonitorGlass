using Microsoft.Extensions.Hosting;

namespace MonitorGlass.Application.Common.Interfaces;

public interface ISystemMetricWorker : IHostedService, IDisposable
{
    Task StartCollectMetricsAsync(CancellationToken cancellationToken = default);
}

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MonitorGlass.Application.Common.Interfaces;

namespace MonitorGlass.Infrastructure.Hubs;

public sealed class WindowsHub(ILogger<WindowsHub> logger) : Hub<IWindowsHub>
{
    private readonly ILogger<WindowsHub> _logger = logger;
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogError(exception, "An error occured on SignalR. \n{message}", exception?.Message);
        return base.OnDisconnectedAsync(exception);
    }
}

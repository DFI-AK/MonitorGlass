using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Interfaces;

public interface IWindowsRepository
{
    Task<WindowsServer?> CreateSystemInfoAsync(WindowsServer systemInfo, CancellationToken cancellationToken = default);
    Task<WindowsServer?> GetSystemInfoByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WindowsServer?> GetSystemInfoByNameAsync(string serverName, CancellationToken cancellationToken = default);
    Task<WindowsServer?> UpdateSystemInfoAsync(WindowsServer systemInfo, CancellationToken cancellationToken = default);
    Task<bool> DeleteSystemInfoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WindowsServer>> GetAllSystemInfosAsync(CancellationToken cancellationToken = default);
    Task<bool> IsExistAsync(string machineName, CancellationToken cancellationToken = default);
}

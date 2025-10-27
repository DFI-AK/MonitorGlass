using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Interfaces;

public interface ISystemInformationRepository
{
    Task<SystemInfo?> CreateSystemInfoAsync(SystemInfo systemInfo, CancellationToken cancellationToken = default);
    Task<SystemInfo?> GetSystemInfoByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SystemInfo?> UpdateSystemInfoAsync(SystemInfo systemInfo, CancellationToken cancellationToken = default);
    Task<bool> DeleteSystemInfoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemInfo>> GetAllSystemInfosAsync(CancellationToken cancellationToken = default);
    Task<bool> IsExistAsync(string machineName, CancellationToken cancellationToken = default);
}

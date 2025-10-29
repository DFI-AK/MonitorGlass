using MonitorGlass.Application.Common.Interfaces;

namespace MonitorGlass.Application.Windows.Commands.AddServer;

public record AddServerCommand : IRequest<Result>
{
    public string? ServerName { get; init; }
    public bool IsRemoteServer { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
}

internal sealed class AddServerCommandHandler(ISystemInformationRepository repository, ISystemProbeService probeService) : IRequestHandler<AddServerCommand, Result>
{
    private readonly ISystemInformationRepository _repository = repository;
    private readonly ISystemProbeService _probeService = probeService;
    public async Task<Result> Handle(AddServerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.ServerName))
        {
            throw new ArgumentNullException("Server name cannot be null or empty.", nameof(request.ServerName));
        }

        var exists = await _repository.IsExistAsync(request.ServerName, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"Server '{request.ServerName}' already exists.");
        }

        var isAvailable = await _probeService.CheckServerAvailabilityAsync(request.ServerName, cancellationToken);
        if (!isAvailable)
        {
            throw new InvalidOperationException($"Server '{request.ServerName}' is not reachable.");
        }
        string operatingSystem = string.Empty;

        if (request.IsRemoteServer && !string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Password))
        {
            operatingSystem = await _probeService.GetOperatingSystemRemoteAsync(request.ServerName, request.UserName, request.Password, cancellationToken);
        }
        else
        {
            operatingSystem = await _probeService.GetLocalOperatingSystemAsync(request.ServerName, cancellationToken);
        }

        var server = new Domain.Entities.SystemInfo
        {
            MachineName = request.ServerName,
            OSVersion = operatingSystem,
        };

        var entity = await _repository.CreateSystemInfoAsync(server, cancellationToken);
        if (entity is null) throw new ApplicationException("Failed to add the server.");
        return Result.Success();
    }
}

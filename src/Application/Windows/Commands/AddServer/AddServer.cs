using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Windows.Commands.AddServer;

public record AddServerCommand : WindowsDto, IRequest<Result>;

internal sealed class AddServerCommandHandler(IWindowsRepository repository, IWindowsProbeService probeService, IMapper mapper) : IRequestHandler<AddServerCommand, Result>
{
    private readonly IWindowsRepository _repository = repository;
    private readonly IWindowsProbeService _probeService = probeService;
    private readonly IMapper _mapper = mapper;
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
        string operatingSystem = await _probeService.GetLocalOperatingSystemAsync(request.ServerName, cancellationToken);

        var server = new WindowsServer
        {
            MachineName = request.ServerName,
            OSVersion = operatingSystem,
        };

        server = _mapper.Map(request, server);

        var entity = await _repository.CreateSystemInfoAsync(_mapper.Map<WindowsServer>(server), cancellationToken);
        return entity is null ? throw new ApplicationException("Failed to add the server.") : Result.Success();
    }
}

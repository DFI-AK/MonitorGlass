namespace MonitorGlass.Application.Windows.Queries.GetWindowsServers;

public record GetWindowsServersQuery : IRequest<WindowsDto>;


internal sealed class GetWindowsServersQueryHandler(IWindowsRepository repository, IMapper mapper) : IRequestHandler<GetWindowsServersQuery, WindowsDto>
{
    private readonly IWindowsRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<WindowsDto> Handle(GetWindowsServersQuery request, CancellationToken cancellationToken)
    {
        var servers = await _repository.GetWindowsServerAsync(cancellationToken) ?? throw new KeyNotFoundException("No server is added yet.");
        return _mapper.Map<WindowsDto>(servers);
    }
}

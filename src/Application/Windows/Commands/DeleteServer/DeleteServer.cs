using MonitorGlass.Application.Common.Interfaces;

namespace MonitorGlass.Application.Windows.Commands.DeleteServer;

public record DeleteServerCommand(Guid ServerId) : IRequest<Result>;

internal sealed class DeleteServerCommandHandler(IWindowsRepository systemInformationRepository) : IRequestHandler<DeleteServerCommand, Result>
{
    private readonly IWindowsRepository _systemInformationRepository = systemInformationRepository;
    public async Task<Result> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        var result = await _systemInformationRepository.DeleteSystemInfoAsync(request.ServerId, cancellationToken);
        return result ? Result.Success() : Result.Failure(["Failed to delete the server."]);
    }
}

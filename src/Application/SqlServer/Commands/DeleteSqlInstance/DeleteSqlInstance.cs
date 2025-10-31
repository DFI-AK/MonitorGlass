using MonitorGlass.Application.Common.Interfaces;

namespace MonitorGlass.Application.SqlServer.Commands.DeleteSqlInstance;

public record DeleteSqlInstanceCommand(Guid InstanceId) : IRequest<Result>
{
}

internal sealed class DeleteSqlInstanceCommandHandler(ISqlServerRepository sqlServerRepository) : IRequestHandler<DeleteSqlInstanceCommand, Result>
{
    private readonly ISqlServerRepository _sqlServerRepository = sqlServerRepository;
    public async Task<Result> Handle(DeleteSqlInstanceCommand request, CancellationToken cancellationToken)
     => await _sqlServerRepository.DeleteAsync(request.InstanceId, cancellationToken);
}

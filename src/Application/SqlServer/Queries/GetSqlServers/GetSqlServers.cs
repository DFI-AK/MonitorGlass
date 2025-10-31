using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Enums;

namespace MonitorGlass.Application.SqlServer.Queries.GetSqlServers;

public record GetSqlServersQuery : IRequest<SqlServerLookup>;

internal sealed class GetSqlServersQueryHandler(ISqlServerRepository repository, IMapper mapper) : IRequestHandler<GetSqlServersQuery, SqlServerLookup>
{
    private readonly ISqlServerRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    public async Task<SqlServerLookup> Handle(GetSqlServersQuery request, CancellationToken cancellationToken)
    {
        var sqlInstances = await _repository.GetAllAsync(cancellationToken);
        var sqlServers = _mapper.Map<IEnumerable<SqlServerDto>>(sqlInstances);
        var status = Enum.GetValues(typeof(SqlServerStatus))
        .Cast<SqlServerStatus>()
        .Select(x => new SqlServerStatusDto() { Id = (int)x, Status = x.ToString() })
        .ToList().AsReadOnly();
        return new()
        {
            SqlServers = sqlServers.ToList().AsReadOnly(),
            Status = status
        };
    }
}

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Entities;


namespace MonitorGlass.Application.SqlServer.Commands.AddNewSqlServer;

public record AddNewSqlServerCommand(string InstanceName) : IRequest<SqlServerDto>;

internal class AddNewSqlServerCommandHandler(ISqlServerRepository repository, ISqlConnectionValidatorService service, ILogger<AddNewSqlServerCommandHandler> logger, IMapper mapper) : IRequestHandler<AddNewSqlServerCommand, SqlServerDto>
{
    private readonly ISqlServerRepository _repository = repository;
    private readonly ISqlConnectionValidatorService _service = service;
    private readonly ILogger<AddNewSqlServerCommandHandler> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task<SqlServerDto> Handle(AddNewSqlServerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.InstanceName)) throw new InvalidOperationException("Instance name is required");
        var sqlConnectionBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = request.InstanceName,
            ConnectTimeout = 30,
            InitialCatalog = "master",
            IntegratedSecurity = true,
            TrustServerCertificate = true
        };

        var connectionString = sqlConnectionBuilder.ConnectionString;

        var instanceValidation = await _service.ValidateInstanceAsync(connectionString);
        if (!instanceValidation.IsValid) throw new ApplicationException($"SQL instance connection failed: {instanceValidation.Error}");

        _logger.LogInformation("Connection successful. Instance: {InstanceName}", instanceValidation.InstanceName);

        var newInstance = new SqlServerInstance
        {
            InstanceName = instanceValidation.InstanceName,
            Version = instanceValidation.Version,
            ConnectionString = connectionString,
            IsConnected = true,
            IsDefault = false,
        };

        await _repository.AddInstanceAsync(newInstance, cancellationToken);
        _logger.LogInformation("SQL Server instance {InstanceName} added successfully.", newInstance.InstanceName);


        return _mapper.Map<SqlServerDto>(newInstance);
    }
}

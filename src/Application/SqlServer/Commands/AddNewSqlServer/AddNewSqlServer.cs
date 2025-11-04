using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MonitorGlass.Domain.Entities;


namespace MonitorGlass.Application.SqlServer.Commands.AddNewSqlServer;

public record AddNewSqlServerCommand : SqlServerDto, IRequest<SqlServerDto?>;

internal sealed partial class AddNewSqlServerCommandHandler(ISqlServerRepository repository, ISqlConnectionValidatorService service, ILogger<AddNewSqlServerCommandHandler> logger, IMapper mapper, IWindowsRepository windowsRepository) : IRequestHandler<AddNewSqlServerCommand, SqlServerDto?>
{
    private readonly ISqlServerRepository _repository = repository;
    private readonly ISqlConnectionValidatorService _service = service;
    private readonly ILogger<AddNewSqlServerCommandHandler> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IWindowsRepository _windowsRepository = windowsRepository;

    public async Task<SqlServerDto?> Handle(AddNewSqlServerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.InstanceName)) throw new InvalidOperationException("Instance name is required");

        var input = request.InstanceName.Trim();

        // Matches optional protocol (tcp:), machine name or IP, optional instance (\something), and optional port (,number)
        var match = SqlServerFilter().Match(input);

        if (!match.Success)
        {
            return null;
        }

        var machineName = match.Groups["machine"].Value;
        var instanceName = match.Groups["instance"].Success ? match.Groups["instance"].Value : null;
        var port = match.Groups["port"].Success ? int.Parse(match.Groups["port"].Value) : (int?)null;


        var windowsServer = await _windowsRepository.GetSystemInfoByNameAsync(machineName, cancellationToken)
            ?? throw new KeyNotFoundException($"Windows server with this name '{machineName}' not found. Please add windows server first.");

        var isExist = await _repository.IsInstanceExistAsync(instanceName ?? machineName, cancellationToken);

        if (isExist) throw new InvalidOperationException($"Instance with this name '{instanceName ?? machineName}' is already added.");

        var sqlConnectionBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = instanceName ?? machineName,
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
            ServerId = windowsServer.Id
        };

        await _repository.AddInstanceAsync(newInstance, cancellationToken);
        _logger.LogInformation("SQL Server instance {InstanceName} added successfully.", newInstance.InstanceName);


        return _mapper.Map<SqlServerDto>(newInstance);
    }

    [GeneratedRegex(@"^(?:tcp:)?(?<machine>[^\\,]+)(?:\\(?<instance>[^,]+))?(?:,(?<port>\d+))?$", RegexOptions.IgnoreCase, "en-IN")]
    private static partial Regex SqlServerFilter();
}

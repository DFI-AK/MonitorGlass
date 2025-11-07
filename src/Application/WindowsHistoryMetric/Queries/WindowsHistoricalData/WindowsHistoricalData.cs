namespace MonitorGlass.Application.WindowsHistoryMetric.Queries.WindowsHistoricalData;

public record WindowsHistoricalDataQuery : IRequest<PaginatedList<WindowsMetricDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
}

internal sealed class WindowsHistoricalDataQueryHandler(IWindowsMetricRepository repository) : IRequestHandler<WindowsHistoricalDataQuery, PaginatedList<WindowsMetricDto>>
{
    private readonly IWindowsMetricRepository _repository = repository;
    public async Task<PaginatedList<WindowsMetricDto>> Handle(WindowsHistoricalDataQuery request, CancellationToken cancellationToken)
     => await _repository.GetHistoricalDataAsync(request.PageNumber, request.PageSize, request.From, request.To, cancellationToken);
}

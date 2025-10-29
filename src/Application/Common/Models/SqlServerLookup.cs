using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Models;

public record SqlServerLookup
{
    public IReadOnlyList<SqlServerDto> SqlServers { get; set; } = [];
    public IReadOnlyList<SqlServerStatusDto> Status { get; set; } = [];
}

public record SqlServerDto
{
    public Guid InstanceId { get; init; }
    public string? InstanceName { get; init; }
    public string? SqlVersion { get; init; }
    public bool IsConnected { get; init; }
    public DateTimeOffset Created { get; init; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SqlServerInstance, SqlServerDto>()
            .ForMember(dest => dest.InstanceId, o => o.MapFrom(src => src.Id))
            .ForMember(dest => dest.SqlVersion, o => o.MapFrom(src => src.Version));
        }
    }
}
public record SqlServerStatusDto
{
    public int Id { get; set; }
    public string? Status { get; set; }
}
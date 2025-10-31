using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Models;

public record SqlServerLookup
{
    public IReadOnlyList<SqlServerDto> SqlServers { get; set; } = [];
    public IReadOnlyList<SqlServerStatusDto> Status { get; set; } = [];
}

public record SqlServerDto
{
    public Guid? InstanceId { get; init; }
    public string? InstanceName { get; init; }
    public string? SqlVersion { get; init; }
    public string? ConnectionString { get; init; }
    public bool IsConnected { get; init; }
    public IReadOnlyCollection<DatabaseDto> Databases { get; init; } = [];

    public DateTimeOffset Created { get; init; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SqlServerInstance, SqlServerDto>()
            .ForMember(dest => dest.InstanceId, o => o.MapFrom(src => src.Id))
            .ForMember(dest => dest.InstanceName, o => o.MapFrom<DecryptValueResolver, string?>(src => src.InstanceName))
            .ForMember(dest => dest.ConnectionString, o => o.MapFrom<DecryptValueResolver, string?>(src => src.ConnectionString))
            .ForMember(dest => dest.SqlVersion, o => o.MapFrom(src => src.Version));

            CreateMap<SqlServerDto, SqlServerInstance>()
            .ForMember(dest => dest.Version, o => o.MapFrom(src => src.SqlVersion))
            .ForMember(dest => dest.InstanceName, o => o.MapFrom<EncryptValueResolver, string?>(src => src.InstanceName))
            .ForMember(dest => dest.ConnectionString, o => o.MapFrom<EncryptValueResolver, string?>(src => src.ConnectionString));
        }
    }
}
public record SqlServerStatusDto
{
    public int Id { get; set; }
    public string? Status { get; set; }
}

public record DatabaseDto
{
    public string? DatabaseName { get; set; }
    public long SizeInMb { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SqlDatabase, DatabaseDto>()
            .ForMember(dest => dest.DatabaseName, o => o.MapFrom<DecryptValueResolver, string?>(src => src.Name))
            .ForMember(dest => dest.SizeInMb, o => o.MapFrom(src => src.SizeMB));

            CreateMap<DatabaseDto, SqlDatabase>()
            .ForMember(dest => dest.Name, o => o.MapFrom<EncryptValueResolver, string?>(src => src.DatabaseName))
            .ForMember(dest => dest.SizeMB, o => o.MapFrom(src => src.SizeInMb));
        }
    }
}

public sealed class DecryptValueResolver(IEncryptionService service)
    : IMemberValueResolver<object, object, string?, string?>
{
    private readonly IEncryptionService _service = service;

    public string? Resolve(
        object source,
        object destination,
        string? sourceMember,
        string? destMember,
        ResolutionContext context)
    {
        return string.IsNullOrEmpty(sourceMember)
            ? string.Empty
            : _service.DecryptData(sourceMember);
    }
}

public sealed class EncryptValueResolver(IEncryptionService service) : IMemberValueResolver<object, object, string?, string?>
{
    private readonly IEncryptionService _service = service;

    public string? Resolve(object source, object destination, string? sourceMember, string? destMember, ResolutionContext context)
        => string.IsNullOrEmpty(destMember)
        ? string.Empty
        : _service.EncryptData(destMember);
}
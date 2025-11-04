using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Models;

public record WindowsDto
{
    public Guid? Id { get; set; }
    public string? ServerName { get; set; }
    public string? OS { get; set; }
    public DateTimeOffset? Created { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<WindowsServer, WindowsDto>()
            .ForMember(dest => dest.ServerName, o => o.MapFrom<DecryptValueResolver, string?>(src => src.MachineName))
            .ForMember(dest => dest.OS, o => o.MapFrom<DecryptValueResolver, string?>(src => src.OSVersion));

            CreateMap<WindowsDto, WindowsServer>()
            .ForMember(dest => dest.MachineName, o => o.MapFrom<EncryptValueResolver, string?>(src => src.ServerName))
            .ForMember(dest => dest.OSVersion, o => o.MapFrom<EncryptValueResolver, string?>(src => src.OS));
        }
    }
}

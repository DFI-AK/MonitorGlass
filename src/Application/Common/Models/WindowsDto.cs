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

public record WindowsMetricDto
{
    public Guid ServerId { get; set; }
    public CpuDto CPU { get; set; } = null!;
    public MemoryDto Memory { get; set; } = null!;
    public IReadOnlyCollection<DiskDto> DiskDetails { get; set; } = [];
    public IReadOnlyCollection<NetworkDto> NetworkDetails { get; set; } = [];
    public DateTimeOffset Created { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<WindowsMetric, WindowsMetricDto>()
            .ForMember(dest => dest.ServerId, o => o.MapFrom(src => src.SystemInfoId))
            .ForMember(dest => dest.CPU, o => o.MapFrom(src => src.CpuDetail))
            .ForMember(dest => dest.Memory, o => o.MapFrom(src => src.MemoryDetail))
            .ForMember(dest => dest.DiskDetails, o => o.MapFrom(src => src.DiskDetails))
            .ForMember(dest => dest.NetworkDetails, o => o.MapFrom(src => src.NetworkDetails));
        }
    }
}

public record CpuDto
{
    public int Cores { get; set; }
    public double CoreUsage { get; set; }
    public int ProcessCount { get; set; }
    public int ThreadCount { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CpuDetail, CpuDto>()
            .ForMember(dest => dest.Cores, o => o.MapFrom(src => src.CpuCores))
            .ForMember(dest => dest.CoreUsage, o => o.MapFrom(src => src.CpuCoreUsage))
            .ForMember(dest => dest.ProcessCount, o => o.MapFrom(src => src.CpuProcessCount))
            .ForMember(dest => dest.ThreadCount, o => o.MapFrom(src => src.CpuThreadCount));
        }
    }
}

public record MemoryDto
{
    public double TotalMemoryMB { get; set; }
    public double UsedMemoryMB { get; set; }
    public double AvailableMemoryMB { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<MemoryDetail, MemoryDto>();
        }
    }
}


public record DiskDto
{
    public string? DriveLetter { get; set; }
    public double? DiskReadSpeedMBps { get; set; }
    public double? DiskWriteSpeedMBps { get; set; }
    public int? DiskIOPS { get; set; }
    public double? DiskFreeSpaceGB { get; set; }
    public double? DiskTotalSpaceGB { get; set; }
    public DateTimeOffset Created { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DiskDetail, DiskDto>();
        }
    }
}

public record NetworkDto
{
    public string InterfaceName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MACAddress { get; set; } = string.Empty;
    public string IPv4Address { get; set; } = string.Empty;
    public string? IPv6Address { get; set; }
    public bool IsUp { get; set; }
    public long SpeedMbps { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public DateTimeOffset Created { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<NetworkDetail, NetworkDto>();
        }
    }
}
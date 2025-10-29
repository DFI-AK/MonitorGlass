using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Application.Common.Models;

public record UserDto
{
    public Guid Id { get; init; }
    public string? DisplayName { get; init; }
    public string? EmailAddress { get; init; }
    public bool EmailVerified { get; init; }
    public string? PhoneNumber { get; init; }
    public bool PhoneNumberVerified { get; init; }
    public List<string> Roles { get; set; } = [];
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.EmailAddress, o => o.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmailVerified, o => o.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.PhoneNumberVerified, o => o.MapFrom(src => src.PhoneNumberConfirmed))
            .ForMember(dest => dest.Roles, o => o.Ignore());
        }
    }
}
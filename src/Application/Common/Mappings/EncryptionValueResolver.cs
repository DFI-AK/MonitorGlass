namespace MonitorGlass.Application.Common.Mappings;


internal sealed class DecryptValueResolver(IEncryptionService service)
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

internal sealed class EncryptValueResolver(IEncryptionService service) : IMemberValueResolver<object, object, string?, string?>
{
    private readonly IEncryptionService _service = service;

    public string? Resolve(object source, object destination, string? sourceMember, string? destMember, ResolutionContext context)
        => string.IsNullOrEmpty(destMember)
        ? string.Empty
        : _service.EncryptData(destMember);
}
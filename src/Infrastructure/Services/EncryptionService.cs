using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace MonitorGlass.Infrastructure.Services;

public sealed class EncryptionService(IConfiguration configuration, IDataProtectionProvider protector)
{
    private readonly IDataProtector _protector = protector.CreateProtector(configuration["Crypto:SecretKey"] ?? throw new ArgumentNullException("Crypto secret key is null or empty"));

    public string EncryptData(string value) => _protector.Protect(value);
    public string DecryptData(string value) => _protector.Unprotect(value);
}

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using MonitorGlass.Application.Common.Interfaces;

namespace MonitorGlass.Infrastructure.Services;

public sealed class EncryptionService(IConfiguration configuration, IDataProtectionProvider protector) : IEncryptionService
{
    private readonly IDataProtector _protector = protector.CreateProtector(configuration["Crypto:SecretKey"] ?? throw new ArgumentNullException("Crypto secret key is null or empty"));

    public string EncryptData(string? value) => string.IsNullOrEmpty(value)
    ? throw new ArgumentNullException(nameof(value), "Value for encryption cannot be null or empty.")
    : _protector.Protect(value);

    public string DecryptData(string? value) => string.IsNullOrEmpty(value)
    ? throw new ArgumentNullException(nameof(value), "Value cannot be null or empty for decryption.")
    : _protector.Unprotect(value);
}

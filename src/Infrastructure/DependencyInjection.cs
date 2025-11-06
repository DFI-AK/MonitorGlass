using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Domain.Constants;
using MonitorGlass.Infrastructure.Data;
using MonitorGlass.Infrastructure.Data.Interceptors;
using MonitorGlass.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MonitorGlass.Domain.Entities;
using Microsoft.AspNetCore.DataProtection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("MonitorGlassDb");
        Guard.Against.Null(connectionString, message: "Connection string 'MonitorGlassDb' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IWindowsRepository, WindowsRepository>();
        builder.Services.AddScoped<IWindowsMetricRepository, WindowsMetricRepository>();
        builder.Services.AddScoped<ISqlServerRepository, SqlServerRepository>();
        builder.Services.AddScoped<SqlServerRepository>();
        builder.Services.AddScoped<SqlServerMonitoringService>();
        builder.Services.AddScoped<SqlServerInstanceService>();
        builder.Services.AddScoped<ISqlConnectionValidatorService, SqlConnectionValidatorService>();
        builder.Services.AddScoped<EncryptionService>();
        builder.Services.AddScoped<IEncryptionService, EncryptionService>();
        builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new(Path.Combine(AppContext.BaseDirectory, "cypherkeys")))
        .SetApplicationName("MonitorGlass");

        if (OperatingSystem.IsWindows())
        {
            builder.Services.AddScoped<IWindowsProbeService, WindowsProbeService>();
            builder.Services.AddHostedService<WindowsMetricWorker>();
        }

        builder.Services.AddAuthentication()
        .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        builder.Services.AddSignalR();
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<List<string>>()
        ?? throw new InvalidOperationException("Allowed origins are missing from appsettings.");

        builder.Services.AddCors(o => o.AddPolicy("AllowedOrigins", p => p.WithOrigins([.. allowedOrigins]).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
    }
}

using Azure.Identity;
using MonitorGlass.Application.Common.Interfaces;
using MonitorGlass.Infrastructure.Data;
using MonitorGlass.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;


namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        builder.Services.AddRazorPages();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "MonitorGlass API";
        });

        builder.Services.AddSerilog((sp, opt) =>
        {
            var scope = sp.CreateAsyncScope().ServiceProvider;
            var environment = scope.GetRequiredService<IWebHostEnvironment>();

            opt.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
            opt.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information);
            opt.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);
            opt.MinimumLevel.Override("Microsoft.AspNetCore.SpaProxy", LogEventLevel.Information);

            if (environment.IsDevelopment())
            {
                opt.WriteTo.Console();
            }
            else
            {
                var path = Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt");
                opt.WriteTo.File(path, rollingInterval: RollingInterval.Day);
            }
        });
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}

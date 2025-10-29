using MonitorGlass.Domain.Constants;
using MonitorGlass.Domain.Entities;
using MonitorGlass.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MonitorGlass.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }

    public async Task SeedRolesAsync()
    {
        _logger.LogInformation("Seeding roles...");
        List<ApplicationRole> roles =
        [
            new(Roles.Administrator,"Can have full access to all features."),
            new(Roles.Operator,"Can manage user accounts and settings."),
            new(Roles.Viewer,"Can view content but cannot make changes."),
            new(Roles.DBA,"Can manage the database and perform backups.")
        ];

        foreach (var role in roles)
        {
            if (!string.IsNullOrEmpty(role.Name) && !await _roleManager.RoleExistsAsync(role.Name))
            {
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Created role '{RoleName}'", role.Name);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Error creating role '{RoleName}': {Errors}", role.Name, errors);
                }
            }
        }
        _logger.LogInformation("Finished seeding roles.");
    }

    async Task SeedAdminUserAsync()
    {
        _logger.LogInformation("Seeding admin user...");

        var adminUser = new ApplicationUser
        {
            UserName = "ayush@localhost",
            Email = "ayush@localhost",
            EmailConfirmed = true,
            DisplayName = "Ayush Kumar",
        };

        try
        {
            if (_userManager.Users.All(x => x.UserName != adminUser.UserName))
            {
                var result = await _userManager.CreateAsync(adminUser, "Test@123");
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created admin user '{UserName}'", adminUser.UserName);

                    await _userManager.AddToRoleAsync(adminUser, Roles.Administrator);

                    _logger.LogInformation("Added admin user '{UserName}' to role '{RoleName}'", adminUser.UserName, Roles.Administrator);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Error creating admin user '{UserName}': {Errors}", adminUser.UserName, errors);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding admin user");
            throw;
        }
    }
}

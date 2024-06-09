using GlowingStoreApplication.Authentication;
using GlowingStoreApplication.Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GlowingStoreApplication.BusinessLayer.StartupServices;

public class IdentityRoleService : IHostedService
{
    private readonly IServiceProvider services;
    private readonly ILogger<IdentityRoleService> logger;

    public IdentityRoleService(IServiceProvider services, ILogger<IdentityRoleService> logger)
    {
        this.services = services;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("starting identity role service");
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var roleNames = new string[] { RoleNames.Administrator, RoleNames.PowerUser, RoleNames.User };

        foreach (var roleName in roleNames)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                var role = new ApplicationRole(roleName)
                {
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                await roleManager.CreateAsync(role);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("stopping identity role service");
        return Task.CompletedTask;
    }
}
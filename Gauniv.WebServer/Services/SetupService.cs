using Gauniv.WebServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Services
{
    public class SetupService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SetupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            // Resolve your services
            var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            if (applicationDbContext == null)
            {
                throw new Exception("ApplicationDbContext is null");
            }

            // 1) Migrate the database if there are pending migrations
            if (applicationDbContext.Database.GetPendingMigrations().Any())
            {
                applicationDbContext.Database.Migrate();
            }

            // 2) Ensure the Admin and User roles exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // 3) (Optional) Seed a default admin user
            //    If you already have an admin user, you can skip this step.
            var existingAdmin = await userManager.FindByNameAsync("admin");
            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    FirstName = "Default",
                    LastName = "Admin"
                };

                // Choose a strong password in real scenarios
                var createAdminResult = await userManager.CreateAsync(adminUser, "AdminPassword123!");

                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Done
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Nothing special to do on stop
            return Task.CompletedTask;
        }
    }
}

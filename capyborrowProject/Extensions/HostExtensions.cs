using Microsoft.AspNetCore.Identity;
using capyborrowProject.Models;
using capyborrowProject.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace capyborrowProject.Extensions
{
    public static class HostExtensions
    {
        public static async Task SeedUserRolesAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                // Seed roles
                var roles = new[] { "student", "teacher", "admin" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }


                var adminEmail = "admin@gmail.com";
                var adminPassword = "admineuni";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        FirstName = "Admin",
                        LastName = "eUni",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    adminUser = new ApplicationUser
                    {
                        FirstName = "Admin",
                        LastName = "eUni",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "admin");

                        await userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.NameIdentifier, adminUser.Id));
                        await userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Email, adminUser.Email));
                        await userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "admin"));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error creating admin user: {error.Description}");
                        }
                    }
                }
            }
        }
    }
}

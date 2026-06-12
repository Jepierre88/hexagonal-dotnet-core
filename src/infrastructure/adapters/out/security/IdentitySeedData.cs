using Domain.Iam;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Adapters.Out.Security;

public static class IdentitySeedData
{
    public static async Task InitializeAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager,
        BootstrapAdminOptions bootstrapAdminOptions)
    {
        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (string.IsNullOrWhiteSpace(bootstrapAdminOptions.Email) || string.IsNullOrWhiteSpace(bootstrapAdminOptions.Password))
        {
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(bootstrapAdminOptions.Email);
        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                UserName = bootstrapAdminOptions.Email,
                Email = bootstrapAdminOptions.Email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, bootstrapAdminOptions.Password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo crear el usuario administrador bootstrap: {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            var addAdminResult = await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
            if (!addAdminResult.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo asignar el rol Admin al usuario bootstrap: {string.Join(", ", addAdminResult.Errors.Select(error => error.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, AppRoles.User))
        {
            var addUserResult = await userManager.AddToRoleAsync(adminUser, AppRoles.User);
            if (!addUserResult.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo asignar el rol User al usuario bootstrap: {string.Join(", ", addUserResult.Errors.Select(error => error.Description))}");
            }
        }
    }
}


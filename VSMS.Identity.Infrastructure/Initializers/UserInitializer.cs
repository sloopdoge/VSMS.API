using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VSMS.Identity.Domain;
using VSMS.Identity.Domain.Entities;
using VSMS.Identity.Domain.Models;

namespace VSMS.Identity.Infrastructure.Initializers;

public static class UserInitializer
{
    private const string AdminPassword = "Password1!";
    private const string AdminEmail = "admin@admin.com";

    public static async Task Initialize(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        try
        {
            var adminUser = await userManager.FindByEmailAsync(AdminEmail);
            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = AdminEmail,
                    EmailConfirmed = true,
                };
                
                var adminUserCreateResult = await userManager.CreateAsync(adminUser, AdminPassword);
                if (!adminUserCreateResult.Succeeded)
                    throw new Exception(string.Join(Environment.NewLine, adminUserCreateResult.Errors));
                
                logger.LogInformation($"Admin user created");
            }

            if (!await userManager.IsInRoleAsync(adminUser, RoleNames.Admin))
            {
                var adminUserAddAdminRoleResult = await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
                if (!adminUserAddAdminRoleResult.Succeeded)
                    throw new Exception(string.Join(Environment.NewLine, adminUserAddAdminRoleResult.Errors));
                
                logger.LogInformation($"{nameof(UserInitializer)}: Added role to Admin user");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}
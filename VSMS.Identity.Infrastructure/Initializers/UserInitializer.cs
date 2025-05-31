using Microsoft.AspNetCore.Identity;
using VSMS.Identity.Domain;
using VSMS.Identity.Domain.Entities;
using VSMS.Identity.Domain.Models;

namespace VSMS.Identity.Infrastructure.Initializers;

public class UserInitializer
{
    private static readonly ApplicationUser AdminUser = new()
    {
        UserName = "admin",
        FirstName = "Admin",
        LastName = "Admin",
        Email = "admin@admin.com",
    };
    
    private const string AdminPassword = "Password1!";

    public static async Task Initialize(UserManager<ApplicationUser> userManager)
    {
        var adminUser = await userManager.FindByEmailAsync(AdminUser.Email!);
        if (adminUser is null)
        {
            var adminUserCreateResult = await userManager.CreateAsync(AdminUser, AdminPassword);
            if (!adminUserCreateResult.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, adminUserCreateResult.Errors));
        }

        if (!await userManager.IsInRoleAsync(AdminUser, RoleNames.Admin))
        {
            var adminUserAddAdminRoleResult = await userManager.AddToRoleAsync(AdminUser, RoleNames.Admin);
            if (!adminUserAddAdminRoleResult.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, adminUserAddAdminRoleResult.Errors));
        }
    }
}
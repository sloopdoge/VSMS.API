using Microsoft.AspNetCore.Identity;
using VSMS.Identity.Domain;
using VSMS.Identity.Domain.Entities;
using VSMS.Identity.Domain.Models;

namespace VSMS.Identity.Infrastructure.Initializers;

public static class RoleInitializer
{
    public static async Task Initialize(RoleManager<ApplicationRole> roleManager)
    {
        foreach (var roleName in RoleNames.All)
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            
            var result = await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            
            if (result.Succeeded) continue;
            
            throw new Exception(string.Join(Environment.NewLine, result.Errors));
        }
    }
}
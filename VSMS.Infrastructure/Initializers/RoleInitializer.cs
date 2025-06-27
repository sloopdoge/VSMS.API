using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VSMS.Domain.Constants;
using VSMS.Domain.Entities;

namespace VSMS.Infrastructure.Initializers;

public static class RoleInitializer
{
    public static async Task Initialize(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        try
        {
            foreach (var roleName in RoleNames.All)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
            
                var result = await roleManager.CreateAsync(new ApplicationRole { Name = roleName });

                if (!result.Succeeded) 
                    throw new Exception(string.Join(Environment.NewLine, result.Errors));
            
                logger.LogInformation($"{nameof(RoleInitializer)}: Role {roleName} created");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}
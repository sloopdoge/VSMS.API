using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VSMS.Identity.Domain.Models;
using VSMS.Identity.Infrastructure.Interfaces;
using VSMS.Identity.Repository;

namespace VSMS.Identity.Infrastructure.Services;

public class UserService(
    ILogger<UserService> logger, 
    ApplicationDbContext context, 
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService) : IUserService
{
    public async Task<ApplicationUser?> GetUserByEmail(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                return null;
            
            var user = await userManager.FindByEmailAsync(email);
            return user;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<bool> IsPasswordCorrect(ApplicationUser user, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
                return false;
            
            var res = await userManager.CheckPasswordAsync(user, password);
            return res;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<Token> GenerateToken(ApplicationUser user, ApplicationRole role, bool useLongLivedToken)
    {
        try
        {
            var res = tokenService.GenerateToken(user, role, useLongLivedToken);
            
            
            return res;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}
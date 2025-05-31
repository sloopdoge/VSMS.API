using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VSMS.Identity.Domain;
using VSMS.Identity.Domain.DTOs;
using VSMS.Identity.Domain.Entities;
using VSMS.Identity.Domain.Models;
using VSMS.Identity.Infrastructure.Interfaces;
using VSMS.Identity.Repository;

namespace VSMS.Identity.Infrastructure.Services;

public class UserService(
    ILogger<UserService> logger, 
    ApplicationDbContext context, 
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
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
    
    public async Task<UserProfileDto> GetUserProfileById(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
                throw new Exception($"User ID is empty");
            
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new Exception($"User not found");
            
            var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            if (userRole is null || string.IsNullOrEmpty(userRole))
                throw new Exception($"User dont have roles");
            
            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = userRole,
            };
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

    public async Task<TokenModel> GenerateToken(ApplicationUser user, bool useLongLivedToken)
    {
        try
        {
            var roleName = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            if (roleName is null || string.IsNullOrEmpty(roleName))
                throw new Exception("User dont have any roles.");
            
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
                throw new Exception("Role doesn't exist.");
            
            return tokenService.GenerateToken(user, role, useLongLivedToken);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<RegisterResultModel> RegisterUser(UserRegisterDto model)
    {
        try
        {
            var userCreateResult = await userManager.CreateAsync(new ApplicationUser
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            }, model.Password);

            if (!userCreateResult.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, userCreateResult.Errors));
            
            var createdUser = await GetUserByEmail(model.Email);
            if (createdUser is null)
                return new RegisterResultModel
                {
                    Success = false,
                    Errors = userCreateResult.Errors
                        .Select(e => e.Description)
                        .ToList()
                };

            var userRoleAddResult = await userManager.AddToRoleAsync(createdUser, RoleNames.User);
            if (!userRoleAddResult.Succeeded)
                return new RegisterResultModel
                {
                    Success = false,
                    Errors = userRoleAddResult.Errors
                        .Select(e => e.Description)
                        .ToList()
                };
            
            var userRole = (await userManager.GetRolesAsync(createdUser)).FirstOrDefault();

            return new RegisterResultModel
            {
                Success = true,
                UserProfile = new UserProfileDto
                {
                    Id = createdUser.Id,
                    Username = createdUser.UserName,
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    Email = createdUser.Email,
                    PhoneNumber = createdUser.PhoneNumber,
                    Role = userRole!,
                },
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}
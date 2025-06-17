using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VSMS.Domain;
using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Domain.Models;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Interfaces;
using VSMS.Repository;

namespace VSMS.Infrastructure.Services;

public class UserService(
    ILogger<UserService> logger, 
    ApplicationDbContext context, 
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ITokenService tokenService) : IUserService
{
    #region Identity
    
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
                throw new Exception("User doesn't have any roles.");
            
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
    
    #endregion
    
    #region Users
    
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
    
    public async Task<ApplicationUser?> GetUserById(Guid id)
    {
        try
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
                throw new UserNotFoundException(id);

            return user;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
    
    #endregion
    
    #region User Profiles
    
    public async Task<UserProfileDto?> GetUserProfileById(Guid userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new UserNotFoundException(userId);

            var roles = await userManager.GetRolesAsync(user);

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "None"
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<UserProfileDto?> CreateUser(UserCreateDto model)
    {
        try
        {
            var createRes = await userManager.CreateAsync(new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
            });
            if (!createRes.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, createRes.Errors));
            
            var createdUser = await userManager.FindByEmailAsync(model.Email);
            if (createdUser is null)
                throw new UserNotFoundException(model.Email);
            
            var roleAssignResult = await userManager.AddToRoleAsync(createdUser, model.RoleName);
            if (!roleAssignResult.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, roleAssignResult.Errors));
            
            var userRole = (await userManager.GetRolesAsync(createdUser)).FirstOrDefault();

            return new UserProfileDto
            {
                Id = createdUser.Id,
                Username = createdUser.UserName,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email,
                PhoneNumber = createdUser.PhoneNumber,
                Role = userRole!,
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<UserProfileDto?> UpdateUserProfile(UserProfileDto updatingUser)
    {
        try
        {
            var user = await userManager.FindByIdAsync(updatingUser.Id.ToString());
            if (user is null)
                throw new UserNotFoundException(updatingUser.Id);

            user.FirstName = updatingUser.FirstName ?? user.FirstName;
            user.LastName = updatingUser.LastName ?? user.LastName;
            user.Email = updatingUser.Email ?? user.Email;
            user.PhoneNumber = updatingUser.PhoneNumber ?? user.PhoneNumber;
            user.UserName = updatingUser.Username ?? user.UserName;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, updateResult.Errors));

            var existingRoles = await userManager.GetRolesAsync(user);
            var currentRole = existingRoles.FirstOrDefault();

            if (!string.Equals(currentRole, updatingUser.Role, StringComparison.OrdinalIgnoreCase))
            {
                if (currentRole is not null)
                {
                    var removeResult = await userManager.RemoveFromRoleAsync(user, currentRole);
                    if (!removeResult.Succeeded)
                        throw new Exception(string.Join(Environment.NewLine, removeResult.Errors));
                }

                var assignResult = await userManager.AddToRoleAsync(user, updatingUser.Role);
                if (!assignResult.Succeeded)
                    throw new Exception(string.Join(Environment.NewLine, assignResult.Errors));
            }

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = updatingUser.Role
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<bool> DeleteUserById(Guid userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new UserNotFoundException(userId);

            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, deleteResult.Errors));

            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<UserProfileDto>> GetAllUserProfiles()
    {
        try
        {
            var users = userManager.Users.ToList();

            var profiles = new List<UserProfileDto>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                profiles.Add(new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "None"
                });
            }

            return profiles;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
    
    #endregion
}
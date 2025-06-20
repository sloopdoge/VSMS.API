﻿using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Domain.Models;

namespace VSMS.Infrastructure.Interfaces;

public interface IUserService
{
    #region Identity
    
    Task<bool> IsPasswordCorrect(ApplicationUser user, string password);
    Task<TokenModel> GenerateToken(ApplicationUser user, bool useLongLivedToken);
    Task<RegisterResultModel> RegisterUser(UserRegisterDto model);
    
    #endregion
    
    
    #region Users
    
    Task<ApplicationUser?> GetUserByEmail(string email);
    Task<List<UserProfileDto>> GetAllUserProfiles();
    Task<ApplicationUser?> GetUserById(Guid id);
    
    #endregion

    #region User Profiles
    
    Task<UserProfileDto?> GetUserProfileById(Guid userId);
    Task<UserProfileDto?> CreateUser(UserCreateDto model);
    Task<UserProfileDto?> UpdateUserProfile(UserProfileDto updatingUser);
    Task<bool> DeleteUserById(Guid userId);
    
    #endregion

}

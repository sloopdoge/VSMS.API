using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Domain.Models;

namespace VSMS.Infrastructure.Interfaces;

public interface IUserService
{
    #region Identity

    /// <summary>
    /// Verifies that the supplied password matches the specified user account.
    /// </summary>
    /// <param name="user">User whose password will be validated.</param>
    /// <param name="password">Plain text password to compare.</param>
    /// <returns><c>true</c> if the password is correct.</returns>
    Task<bool> IsPasswordCorrect(ApplicationUser user, string password);

    /// <summary>
    /// Produces a JWT token for the given user.
    /// </summary>
    /// <param name="user">The application user.</param>
    /// <param name="useLongLivedToken">If set, the token will have a longer expiration time.</param>
    /// <returns>The generated <see cref="TokenModel"/>.</returns>
    Task<TokenModel> GenerateToken(ApplicationUser user, bool useLongLivedToken);

    /// <summary>
    /// Creates a new user account and associated profile using the supplied registration model.
    /// </summary>
    /// <param name="model">Registration details for the user.</param>
    /// <returns>A <see cref="RegisterResultModel"/> indicating success or failure.</returns>
    Task<RegisterResultModel> RegisterUser(UserRegisterDto model);
    
    #endregion
    
    #region Users

    /// <summary>
    /// Gets a user entity by its email address.
    /// </summary>
    /// <param name="email">Email of the user.</param>
    /// <returns>The <see cref="ApplicationUser"/> if found; otherwise <c>null</c>.</returns>
    Task<ApplicationUser?> GetUserByEmail(string email);

    /// <summary>
    /// Retrieves every user profile that exists in the system.
    /// </summary>
    /// <returns>List of all <see cref="UserProfileDto"/>.</returns>
    Task<List<UserProfileDto>> GetAllUserProfiles();

    /// <summary>
    /// Finds a user entity by its unique identifier.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <returns>The <see cref="ApplicationUser"/> if found; otherwise <c>null</c>.</returns>
    Task<ApplicationUser?> GetUserById(Guid userId);
    
    #endregion

    #region User Profiles

    /// <summary>
    /// Retrieves profile information for the given user identifier.
    /// </summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <returns>The <see cref="UserProfileDto"/> if found; otherwise <c>null</c>.</returns>
    Task<UserProfileDto?> GetUserProfileById(Guid userId);

    /// <summary>
    /// Creates a new user profile from the provided details.
    /// </summary>
    /// <param name="model">Information about the user to create.</param>
    /// <returns>The created <see cref="UserProfileDto"/> if successful.</returns>
    Task<UserProfileDto?> CreateUser(UserCreateDto model);

    /// <summary>
    /// Updates profile information for an existing user.
    /// </summary>
    /// <param name="updatingUser">Profile data with updated values.</param>
    /// <returns>The updated <see cref="UserProfileDto"/> if successful.</returns>
    Task<UserProfileDto?> UpdateUserProfile(UserProfileDto updatingUser);

    /// <summary>
    /// Removes a user account and all related data.
    /// </summary>
    /// <param name="userId">Identifier of the user to delete.</param>
    /// <returns><c>true</c> if the user was deleted.</returns>
    Task<bool> DeleteUserById(Guid userId);
    
    #endregion

}

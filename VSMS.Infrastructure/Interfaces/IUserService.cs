using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Domain.Models;

namespace VSMS.Infrastructure.Interfaces;

public interface IUserService
{
    #region Identity

    /// <summary>
    /// Checks whether the provided password is correct for the user.
    /// </summary>
    /// <param name="user">User to verify.</param>
    /// <param name="password">Password to check.</param>
    /// <returns><c>true</c> if the password matches.</returns>
    Task<bool> IsPasswordCorrect(ApplicationUser user, string password);

    /// <summary>
    /// Generates a token for the specified user.
    /// </summary>
    /// <param name="user">The application user.</param>
    /// <param name="useLongLivedToken">Indicates whether to create a long-lived token.</param>
    /// <returns>The generated <see cref="TokenModel"/>.</returns>
    Task<TokenModel> GenerateToken(ApplicationUser user, bool useLongLivedToken);

    /// <summary>
    /// Registers a new user using the provided model.
    /// </summary>
    /// <param name="model">Registration data for the user.</param>
    /// <returns>Result of the registration process.</returns>
    Task<RegisterResultModel> RegisterUser(UserRegisterDto model);
    
    #endregion
    
    #region Users

    /// <summary>
    /// Retrieves a user entity by email.
    /// </summary>
    /// <param name="email">Email of the user.</param>
    /// <returns>The <see cref="ApplicationUser"/> if found; otherwise <c>null</c>.</returns>
    Task<ApplicationUser?> GetUserByEmail(string email);

    /// <summary>
    /// Gets all user profiles in the system.
    /// </summary>
    /// <returns>List of all <see cref="UserProfileDto"/>.</returns>
    Task<List<UserProfileDto>> GetAllUserProfiles();

    /// <summary>
    /// Retrieves a user entity by identifier.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns>The <see cref="ApplicationUser"/> if found; otherwise <c>null</c>.</returns>
    Task<ApplicationUser?> GetUserById(Guid id);
    
    #endregion

    #region User Profiles

    /// <summary>
    /// Retrieves a user profile by user identifier.
    /// </summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <returns>The <see cref="UserProfileDto"/> if found; otherwise <c>null</c>.</returns>
    Task<UserProfileDto?> GetUserProfileById(Guid userId);

    /// <summary>
    /// Creates a new user profile.
    /// </summary>
    /// <param name="model">Information about the user to create.</param>
    /// <returns>The created <see cref="UserProfileDto"/> if successful.</returns>
    Task<UserProfileDto?> CreateUser(UserCreateDto model);

    /// <summary>
    /// Updates an existing user profile.
    /// </summary>
    /// <param name="updatingUser">Profile data with updated values.</param>
    /// <returns>The updated <see cref="UserProfileDto"/> if successful.</returns>
    Task<UserProfileDto?> UpdateUserProfile(UserProfileDto updatingUser);

    /// <summary>
    /// Deletes a user by identifier.
    /// </summary>
    /// <param name="userId">Identifier of the user to delete.</param>
    /// <returns><c>true</c> if the user was deleted.</returns>
    Task<bool> DeleteUserById(Guid userId);
    
    #endregion

}

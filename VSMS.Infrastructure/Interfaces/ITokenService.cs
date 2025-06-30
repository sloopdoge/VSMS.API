using VSMS.Domain.Entities;
using VSMS.Domain.Models;

namespace VSMS.Infrastructure.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token that represents the specified user and role.
    /// </summary>
    /// <param name="user">The authenticated application user.</param>
    /// <param name="role">Role assigned to the user.</param>
    /// <param name="rememberMe">When set to <c>true</c>, creates a token with an extended lifetime.</param>
    /// <returns>The resulting <see cref="TokenModel"/>.</returns>
    TokenModel GenerateToken(ApplicationUser user, ApplicationRole role, bool rememberMe);

    /// <summary>
    /// Validates the provided JWT token and returns the corresponding validation result.
    /// </summary>
    /// <param name="token">Encoded JWT token string.</param>
    /// <returns>A <see cref="TokenValidationResultModel"/> describing the validation outcome.</returns>
    TokenValidationResultModel ValidateToken(string token);
}
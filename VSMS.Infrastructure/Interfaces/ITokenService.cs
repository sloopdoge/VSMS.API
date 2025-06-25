using System.Security.Claims;
using VSMS.Domain.Entities;
using VSMS.Domain.Models;

namespace VSMS.Infrastructure.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Generates an authentication token for the specified user and role.
    /// </summary>
    /// <param name="user">The application user.</param>
    /// <param name="role">Role assigned to the user.</param>
    /// <param name="rememberMe">Indicates if a long-lived token should be generated.</param>
    /// <returns>The created <see cref="TokenModel"/>.</returns>
    TokenModel GenerateToken(ApplicationUser user, ApplicationRole role, bool rememberMe);

    /// <summary>
    /// Validates the provided token and returns its claims principal if valid.
    /// </summary>
    /// <param name="token">Token to validate.</param>
    /// <returns>The token's <see cref="ClaimsPrincipal"/> when validation succeeds; otherwise <c>null</c>.</returns>
    ClaimsPrincipal? ValidateToken(string token);
}
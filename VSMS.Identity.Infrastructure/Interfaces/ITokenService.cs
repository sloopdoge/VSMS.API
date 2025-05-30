using System.Security.Claims;
using VSMS.Identity.Domain.Models;

namespace VSMS.Identity.Infrastructure.Interfaces;

public interface ITokenService
{
    Token GenerateToken(ApplicationUser user, ApplicationRole role, bool rememberMe);
    ClaimsPrincipal? ValidateToken(string token);
}
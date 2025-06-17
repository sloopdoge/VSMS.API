using System.Security.Claims;
using VSMS.Domain.Entities;
using VSMS.Domain.Models;

namespace VSMS.Infrastructure.Interfaces;

public interface ITokenService
{
    TokenModel GenerateToken(ApplicationUser user, ApplicationRole role, bool rememberMe);
    ClaimsPrincipal? ValidateToken(string token);
}
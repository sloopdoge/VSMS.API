using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VSMS.Identity.Domain.Entities;
using VSMS.Identity.Domain.Models;
using VSMS.Identity.Infrastructure.Interfaces;

namespace VSMS.Identity.Infrastructure.Services;

public class TokenService(ILogger<TokenService> logger, IConfiguration configuration) : ITokenService
{
    public TokenModel GenerateToken(ApplicationUser user, ApplicationRole role, bool rememberMe)
    {
        try
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var expiresIn = jwtSettings.GetValue<int>("ExpiresInMinutes");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, role.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetValue<string>("Issuer"),
                audience: jwtSettings.GetValue<string>("Audience"),
                claims: claims,
                expires: rememberMe 
                    ? DateTime.UtcNow.AddMinutes(expiresIn) 
                    : DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new()
            {
                Value = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = rememberMe 
                    ? DateTime.UtcNow.AddMinutes(expiresIn) 
                    : DateTime.UtcNow.AddMinutes(30)
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }

            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token validation failed.");
            return null;
        };
    }
}
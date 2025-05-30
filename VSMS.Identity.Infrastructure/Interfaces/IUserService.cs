using VSMS.Identity.Domain.Models;

namespace VSMS.Identity.Infrastructure.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetUserByEmail(string email);
    Task<bool> IsPasswordCorrect(ApplicationUser user, string password);
    Task<Token> GenerateToken(ApplicationUser user, ApplicationRole role, bool useLongLivedToken);
}
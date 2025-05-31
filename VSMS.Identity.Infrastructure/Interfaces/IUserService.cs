using VSMS.Identity.Domain.DTOs;
using VSMS.Identity.Domain.Entities;
using VSMS.Identity.Domain.Models;

namespace VSMS.Identity.Infrastructure.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetUserByEmail(string email);
    Task<UserProfileDto> GetUserProfileById(Guid userId);
    Task<bool> IsPasswordCorrect(ApplicationUser user, string password);
    Task<TokenModel> GenerateToken(ApplicationUser user, bool useLongLivedToken);
    Task<RegisterResultModel> RegisterUser(UserRegisterDto model);
}
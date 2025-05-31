using VSMS.Identity.Domain.DTOs;

namespace VSMS.Identity.Domain.Models;

public class LoginResultModel
{
    public bool Success { get; set; }
    public TokenModel? Token { get; set; }
    public UserProfileDto? UserProfile { get; set; }
    public List<string>? Errors { get; set; }
}
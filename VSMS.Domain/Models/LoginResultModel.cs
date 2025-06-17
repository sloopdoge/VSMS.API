using VSMS.Domain.DTOs;

namespace VSMS.Domain.Models;

public class LoginResultModel
{
    public bool Success { get; set; }
    public TokenModel? Token { get; set; }
    public UserProfileDto? UserProfile { get; set; }
    public List<string>? Errors { get; set; }
}
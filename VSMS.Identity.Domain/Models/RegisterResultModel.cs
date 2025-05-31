using VSMS.Identity.Domain.DTOs;

namespace VSMS.Identity.Domain.Models;

public class RegisterResultModel
{
    public bool Success { get; set; }
    public UserProfileDto? UserProfile { get; set; }
    public List<string>? Errors { get; set; }
}
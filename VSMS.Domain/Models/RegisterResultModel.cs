using VSMS.Domain.DTOs;

namespace VSMS.Domain.Models;

public class RegisterResultModel
{
    public bool Success { get; set; }
    public UserProfileDto? UserProfile { get; set; }
    public List<string>? Errors { get; set; }
}
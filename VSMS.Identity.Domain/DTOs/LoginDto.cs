namespace VSMS.Identity.Domain.DTOs;

public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool UseLongLivedToken { get; set; } = false;
}
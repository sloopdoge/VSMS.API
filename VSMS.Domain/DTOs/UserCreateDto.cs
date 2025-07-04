using VSMS.Domain.Constants;

namespace VSMS.Domain.DTOs;

public class UserCreateDto
{
    public required string Username {get; set;}
    public required string Email { get; set; }
    public required string RoleName { get; set; } = RoleNames.User;
}
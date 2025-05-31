using VSMS.Identity.Domain.Models;

namespace VSMS.Identity.Domain.DTOs;

public class UserProfileDto
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public required string Email { get; set; }
    public string PhoneNumber { get; set; }
    public required string Role { get; set; }
}
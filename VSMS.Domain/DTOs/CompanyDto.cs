using System.ComponentModel.DataAnnotations;

namespace VSMS.Domain.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<UserProfileDto> UserProfiles { get; set; } = new();
}
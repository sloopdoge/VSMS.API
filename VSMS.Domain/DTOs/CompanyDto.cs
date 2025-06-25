namespace VSMS.Domain.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<UserProfileDto> UserProfiles { get; set; }
}
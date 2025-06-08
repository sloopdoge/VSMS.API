namespace VSMS.Company.Domain.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
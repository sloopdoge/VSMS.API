using Microsoft.EntityFrameworkCore;

namespace VSMS.Domain.Entities;

[PrimaryKey(nameof(Id))]
public class Company
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string NormalizedTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
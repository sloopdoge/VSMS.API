using Microsoft.EntityFrameworkCore;

namespace VSMS.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string NormalizedTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ApplicationUser> Users { get; set; } = [];
    public ICollection<Stock> Stocks { get; set; } = [];
}
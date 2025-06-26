namespace VSMS.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string NormalizedTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ApplicationUser> Users { get; set; } = [];
    public virtual ICollection<Stock> Stocks { get; set; } = [];
}
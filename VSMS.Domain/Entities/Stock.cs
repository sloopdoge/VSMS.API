using VSMS.Domain.Enums;

namespace VSMS.Domain.Entities;

public class Stock
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string NormalizedTitle { get; set; }
    public string Symbol { get; set; }
    public string NormalizedSymbol { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }
    public StockStateEnum State { get; set; }
}
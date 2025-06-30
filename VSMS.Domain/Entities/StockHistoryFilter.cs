namespace VSMS.Domain.Entities;

public class StockHistoryFilter
{
    public DateTime From { get; set; } = DateTime.UtcNow - TimeSpan.FromDays(7);
    public DateTime To { get; set; } = DateTime.UtcNow;
}
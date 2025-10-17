namespace VSMS.Domain.Models.Filters;

public class StocksFilterModel : BaseFilterModel
{
    /// <summary>
    /// Optional filter by related company identifier.
    /// </summary>
    public Guid? CompanyId { get; set; }

    /// <summary>
    /// Optional lower bound for stock price.
    /// </summary>
    public decimal? PriceFrom { get; set; }

    /// <summary>
    /// Optional upper bound for stock price.
    /// </summary>
    public decimal? PriceTo { get; set; }

    /// <summary>
    /// Optional filter by stock title (partial match).
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Optional filter by stock symbol (partial match).
    /// </summary>
    public string? Symbol { get; set; }
}

namespace VSMS.Domain.Models.Filters;

/// <summary>
/// Represents a filtering model for querying company data.
/// </summary>
public class CompaniesFilterModel : BaseFilterModel
{
    /// <summary>
    /// Filters companies by creation date range (optional).
    /// </summary>
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// Filters by partial company name match (overrides Search if both are set).
    /// </summary>
    public string? Title { get; set; }
}
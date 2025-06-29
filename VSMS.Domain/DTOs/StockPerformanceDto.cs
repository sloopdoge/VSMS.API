namespace VSMS.Domain.DTOs;

public class StockPerformanceDto : StockDto
{
    public decimal? PreviousPrice { get; set; }
}
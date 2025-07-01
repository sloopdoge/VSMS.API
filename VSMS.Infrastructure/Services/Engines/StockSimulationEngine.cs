using VSMS.Domain.Entities;
using VSMS.Domain.Enums;
using VSMS.Infrastructure.Interfaces.Engines;

namespace VSMS.Infrastructure.Services.Engines;

public class StockSimulationEngine : IStockSimulationEngine
{
    private readonly Random _random = new();

    public void UpdateStockState(Stock stock)
    {
        stock.State = stock.State switch
        {
            StockStateEnum.Stable => RandomBool() ? StockStateEnum.Rising : StockStateEnum.Falling,
            StockStateEnum.Rising => RandomBool() ? StockStateEnum.Rising : StockStateEnum.Stable,
            StockStateEnum.Falling => RandomBool() ? StockStateEnum.Falling : StockStateEnum.Stable,
            StockStateEnum.Volatile => RandomBool() ? StockStateEnum.Rising : StockStateEnum.Falling,
            _ => StockStateEnum.Stable
        };
    }

    public void UpdateStockPrice(Stock stock)
    {
        decimal changePercent = stock.State switch
        {
            StockStateEnum.Stable => GetRandomChange(0.001m, 0.005m),
            StockStateEnum.Rising => GetRandomChange(0.01m, 0.05m),
            StockStateEnum.Falling => -GetRandomChange(0.01m, 0.04m),
            StockStateEnum.Volatile => RandomBool() ? GetRandomChange(0.05m, 0.1m) : -GetRandomChange(0.05m, 0.1m),
            _ => 0
        };
        
        stock.Price = Math.Max(0.01m, stock.Price * (1 + changePercent));
        stock.UpdatedAt = DateTime.UtcNow;
    }

    private bool RandomBool() => _random.NextDouble() > 0.5;

    private decimal GetRandomChange(decimal min, decimal max)
    {
        double range = (double)(max - min);
        return min + (decimal)(_random.NextDouble() * range);
    }
}
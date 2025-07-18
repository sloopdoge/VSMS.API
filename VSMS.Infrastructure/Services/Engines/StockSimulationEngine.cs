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
            StockStateEnum.Rising => RandomBool(0.7) ? StockStateEnum.Rising : StockStateEnum.Stable,
            StockStateEnum.Falling => RandomBool(0.7) ? StockStateEnum.Falling : StockStateEnum.Stable,
            StockStateEnum.Volatile => RandomBool() ? StockStateEnum.Rising : StockStateEnum.Falling,
            _ => StockStateEnum.Stable
        };
    }

    public void UpdateStockPrice(Stock stock)
    {
        decimal changePercent = stock.State switch
        {
            StockStateEnum.Stable => GetRandomChange(0.0005m, 0.003m),
            StockStateEnum.Rising => GetRandomChange(0.003m, 0.015m),
            StockStateEnum.Falling => -GetRandomChange(0.003m, 0.015m),
            StockStateEnum.Volatile => RandomBool()
                ? GetRandomChange(0.02m, 0.05m)
                : -GetRandomChange(0.02m, 0.05m),
            _ => 0
        };

        decimal maxDeviationFactor = 2.0m;
        var upperBound = stock.InitialPrice * maxDeviationFactor;
        var lowerBound = stock.InitialPrice / maxDeviationFactor;

        var newPrice = stock.Price * (1 + changePercent);

        if (newPrice > upperBound)
        {
            newPrice = stock.Price * (1 - Math.Abs(changePercent));
        }
        else if (newPrice < lowerBound)
        {
            newPrice = stock.Price * (1 + Math.Abs(changePercent));
        }

        stock.Price = Math.Max(0.01m, newPrice);
        stock.UpdatedAt = DateTime.UtcNow;
    }

    private bool RandomBool(double trueProbability = 0.5) => _random.NextDouble() < trueProbability;

    private decimal GetRandomChange(decimal min, decimal max)
    {
        double range = (double)(max - min);
        return min + (decimal)(_random.NextDouble() * range);
    }
}

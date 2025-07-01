using VSMS.Domain.Entities;

namespace VSMS.Infrastructure.Interfaces.Engines;

public interface IStockSimulationEngine
{
    void UpdateStockState(Stock stock);
    void UpdateStockPrice(Stock stock);
}
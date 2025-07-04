using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces.SenderHubs;

public interface IStocksSenderHub
{
    Task OnStocksPriceChanged(List<StockDto> stocks);
}
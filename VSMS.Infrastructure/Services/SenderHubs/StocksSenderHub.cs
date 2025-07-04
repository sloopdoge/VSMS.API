using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using VSMS.Domain.DTOs;
using VSMS.Infrastructure.Hubs;
using VSMS.Infrastructure.Interfaces.SenderHubs;

namespace VSMS.Infrastructure.Services.SenderHubs;

public class StocksSenderHub(
    ILogger<StocksSenderHub> logger,
    IHubContext<StocksHub> stocksHub) : IStocksSenderHub
{
    public async Task OnStocksPriceChanged(List<StockDto> stocks)
    {
        try
        {
            await stocksHub.Clients.All.SendAsync($"{nameof(OnStocksPriceChanged)}",  stocks);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}
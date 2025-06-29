using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using VSMS.Domain;
using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Infrastructure.Hubs;

[Authorize]
public class StocksHub(
    ILogger<StocksHub> logger,
    IStocksService stocksService) : BaseHub(logger)
{
    public async Task<ResponseModel<List<StockDto>>> GetStockHistoryById(Guid id, DateTime startDate, DateTime endDate)
    {
        var response = new ResponseModel<List<StockDto>>()
        {
            Succeeded = false,
        };

        try
        {
            var stockHistory = await stocksService.GetHistoryById(id, startDate, endDate);
            
            response.Succeeded = true;
            response.Value = stockHistory;
            
            return response;
        }
        catch (Exception e)
        {
            response.Succeeded = false;
            response.Error = new Error
            {
                Property = nameof(GetStockHistoryById),
                Description = [e.Message]
            };
            logger.LogError(e, e.Message);
            
            return response;
        }
    }
}
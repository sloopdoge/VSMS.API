using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VSMS.Domain.DTOs;
using VSMS.Infrastructure.Interfaces;
using VSMS.Repository;

namespace VSMS.Infrastructure.Services;

public class StocksService(
    ILogger<StocksService> logger,
    StocksRepository stocksRepository) : IStocksService
{
    public async Task<StockDto> Create(StockDto stock)
    {
        throw new NotImplementedException();
    }

    public async Task<StockDto> Update(StockDto stock)
    {
        throw new NotImplementedException();
    }

    public async Task<StockDto> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<StockDto> GetByTitle(string title)
    {
        throw new NotImplementedException();
    }

    public async Task<List<StockDto>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteById(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<bool> IsTitleExists(string title)
    {
        try
        {
            var normalizedTitle = title.Normalize();
            var result = await stocksRepository.Stocks.Where(c => c.NormalizedTitle == normalizedTitle).FirstOrDefaultAsync();
            
            return result is not null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}
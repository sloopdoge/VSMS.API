using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Extensions;
using VSMS.Infrastructure.Interfaces;
using VSMS.Repository;

namespace VSMS.Infrastructure.Services;

public class StocksService(
    ILogger<StocksService> logger,
    ApplicationRepository repository) : IStocksService
{
    public async Task<StockDto> Create(StockDto stock)
    {
        try
        {
            var newStock = new Stock
            {
                Id = Guid.NewGuid(),
                Title = stock.Title,
                NormalizedTitle = stock.Title.NormalizeText(),
                Symbol = stock.Symbol,
                NormalizedSymbol = stock.Symbol.NormalizeText(),
                Price = stock.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = stock.CompanyId
            };

            var createResult = await repository.Stocks.AddAsync(newStock);
            var result = await repository.SaveChangesAsync();
            if (result < 1)
                throw new Exception($"Stock: {newStock.Title} - was not created");

            return new StockDto
            {
                Id = createResult.Entity.Id,
                Title = createResult.Entity.Title,
                Symbol = createResult.Entity.Symbol,
                Price = createResult.Entity.Price,
                CreatedAt = createResult.Entity.CreatedAt,
                UpdatedAt = createResult.Entity.UpdatedAt,
                CompanyId = createResult.Entity.CompanyId
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<StockDto> Update(StockDto stock)
    {
        try
        {
            var existingStock = await repository.Stocks.FindAsync(stock.Id);
            if (existingStock is null)
                throw new StockNotFoundException(stock.Id);

            existingStock.Title = stock.Title;
            existingStock.NormalizedTitle = stock.Title.NormalizeText();
            existingStock.Symbol = stock.Symbol;
            existingStock.NormalizedSymbol = stock.Symbol.NormalizeText();
            existingStock.Price = stock.Price;
            existingStock.UpdatedAt = DateTime.UtcNow;
            existingStock.CompanyId = stock.CompanyId;

            repository.Stocks.Update(existingStock);
            var result = await repository.SaveChangesAsync();
            if (result < 1)
                throw new Exception($"Stock: {stock.Id} - was not updated");

            return new StockDto
            {
                Id = existingStock.Id,
                Title = existingStock.Title,
                Symbol = existingStock.Symbol,
                Price = existingStock.Price,
                CreatedAt = existingStock.CreatedAt,
                UpdatedAt = existingStock.UpdatedAt,
                CompanyId = existingStock.CompanyId
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<StockDto> GetById(Guid id)
    {
        try
        {
            var stock = await repository.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stock is null)
                throw new StockNotFoundException(id);

            return new StockDto
            {
                Id = stock.Id,
                Title = stock.Title,
                Symbol = stock.Symbol,
                Price = stock.Price,
                CreatedAt = stock.CreatedAt,
                UpdatedAt = stock.UpdatedAt,
                CompanyId = stock.CompanyId
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<StockDto> GetByTitle(string title)
    {
        try
        {
            var normalizedTitle = title.NormalizeText();
            var stock = await repository.Stocks.FirstOrDefaultAsync(s => s.NormalizedTitle == normalizedTitle);
            if (stock is null)
                throw new Exception($"Stock with title '{title}' was not found.");

            return new StockDto
            {
                Id = stock.Id,
                Title = stock.Title,
                Symbol = stock.Symbol,
                Price = stock.Price,
                CreatedAt = stock.CreatedAt,
                UpdatedAt = stock.UpdatedAt,
                CompanyId = stock.CompanyId
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<StockDto>> GetAll()
    {
        try
        {
            var stocks = await repository.Stocks.ToListAsync();
            return stocks.Select(s => new StockDto
            {
                Id = s.Id,
                Title = s.Title,
                Symbol = s.Symbol,
                Price = s.Price,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CompanyId = s.CompanyId
            }).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<bool> DeleteById(Guid id)
    {
        try
        {
            var stock = repository.Stocks.Local.FirstOrDefault(s => s.Id == id)
                        ?? await repository.Stocks.FindAsync(id);
            if (stock is null)
                throw new StockNotFoundException(id);

            repository.Stocks.Remove(stock);
            var result = await repository.SaveChangesAsync();
            return result > 0;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<StockDto>> GetByCompanyId(Guid companyId)
    {
        try
        {
            var stocks = await repository.Stocks
                .Where(s => s.CompanyId == companyId)
                .Select(s => new StockDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Symbol = s.Symbol,
                    Price = s.Price,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    CompanyId = s.CompanyId
                }).ToListAsync();
            return stocks;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<StockPerformanceDto> GetStockPerformanceById(Guid stockId)
    {
        try
        {
            var currentStock = await repository.Stocks
                .FirstOrDefaultAsync(s => s.Id == stockId);
            if (currentStock is null)
                throw new StockNotFoundException(stockId);
            
            var previousStock = await repository.Stocks
                .TemporalAll()
                .Where(s => s.Id == currentStock.Id)
                .OrderByDescending(s => EF.Property<DateTime>(s, "PeriodEnd"))
                .FirstOrDefaultAsync();
            
            return new StockPerformanceDto
            {
                Id = currentStock.Id,
                Title = currentStock.Title,
                Symbol = currentStock.Symbol,
                Price = currentStock.Price,
                CreatedAt = currentStock.CreatedAt,
                UpdatedAt = currentStock.UpdatedAt,
                CompanyId = currentStock.CompanyId,
                PreviousPrice = previousStock?.Price
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<StockPerformanceDto>> GetAllStocksPerformance()
    {
        try
        {
            var stocksPerformance = new List<StockPerformanceDto>();
            
            var currentStocks = await repository.Stocks.ToListAsync();
            
            foreach (var currentStock in currentStocks)
            {
                var previous = await repository.Stocks
                    .TemporalAll()
                    .Where(s => s.Id == currentStock.Id)
                    .OrderByDescending(s => EF.Property<DateTime>(s, "PeriodEnd"))
                    .Skip(1)
                    .FirstOrDefaultAsync();
                
                stocksPerformance.Add(new StockPerformanceDto
                {
                    Id = currentStock.Id,
                    Title = currentStock.Title,
                    Symbol = currentStock.Symbol,
                    Price = currentStock.Price,
                    CreatedAt = currentStock.CreatedAt,
                    UpdatedAt = currentStock.UpdatedAt,
                    CompanyId = currentStock.CompanyId,
                    PreviousPrice = previous?.Price
                });
            }
            
            return stocksPerformance;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<StockPerformanceDto>> GetStocksPerformanceByCompanyId(Guid companyId)
    {
        try
        {
            var stocksPerformance = new List<StockPerformanceDto>();

            var currentStocks = await repository.Stocks
                .Where(s => s.CompanyId == companyId)
                .ToListAsync();
            
            foreach (var currentStock in currentStocks)
            {
                var previous = await repository.Stocks
                    .TemporalAll()
                    .Where(s => s.Id == currentStock.Id && s.CompanyId == currentStock.CompanyId)
                    .OrderByDescending(s => EF.Property<DateTime>(s, "PeriodEnd"))
                    .FirstOrDefaultAsync();
                
                stocksPerformance.Add(new StockPerformanceDto
                {
                    Id = currentStock.Id,
                    Title = currentStock.Title,
                    Symbol = currentStock.Symbol,
                    Price = currentStock.Price,
                    CreatedAt = currentStock.CreatedAt,
                    UpdatedAt = currentStock.UpdatedAt,
                    CompanyId = currentStock.CompanyId,
                    PreviousPrice = previous?.Price
                });
            }
            
            return stocksPerformance;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<StockDto>?> GetHistoryById(Guid stockId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var stock = await repository.Stocks
                .FirstOrDefaultAsync(s => s.Id == stockId);
            if (stock is null)
                throw new StockNotFoundException(stockId);
            
            var stockHistory = await repository.Stocks
                .TemporalFromTo(startDate, endDate)
                .Where(s => s.Id == stock.Id)
                .OrderByDescending(s => EF.Property<DateTime>(s, "PeriodEnd"))
                .ToListAsync();
            
            return stockHistory.Select(s => new StockDto
            {
                Id = s.Id,
                Title = s.Title,
                Symbol = s.Symbol,
                Price = s.Price,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CompanyId = s.CompanyId
            }).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<bool> IsTitleExists(string title)
    {
        try
        {
            var normalizedTitle = title.NormalizeText();
            var result = await repository.Stocks
                .Where(c => c.NormalizedTitle == normalizedTitle)
                .FirstOrDefaultAsync();
            
            return result is not null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}
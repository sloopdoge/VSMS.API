using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces;

public interface IStocksService
{
    Task<StockDto> Create(StockDto stock);
    Task<StockDto> Update(StockDto stock);
    Task<StockDto> GetById(Guid id);
    Task<StockDto> GetByTitle(string title);
    Task<List<StockDto>> GetAll();
    Task<bool> DeleteById(Guid id);
}
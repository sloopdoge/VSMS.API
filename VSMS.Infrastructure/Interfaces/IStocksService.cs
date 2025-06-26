using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces;

public interface IStocksService
{
    /// <summary>
    /// Creates a new stock using the provided data.
    /// </summary>
    /// <param name="stock">Data for the stock to create.</param>
    /// <returns>The created <see cref="StockDto"/>.</returns>
    Task<StockDto> Create(StockDto stock);

    /// <summary>
    /// Updates an existing stock.
    /// </summary>
    /// <param name="stock">The stock data with updated values.</param>
    /// <returns>The updated <see cref="StockDto"/>.</returns>
    Task<StockDto> Update(StockDto stock);

    /// <summary>
    /// Retrieves a stock by identifier.
    /// </summary>
    /// <param name="id">Stock identifier.</param>
    /// <returns>The <see cref="StockDto"/> if found.</returns>
    Task<StockDto> GetById(Guid id);

    /// <summary>
    /// Retrieves a stock by its title.
    /// </summary>
    /// <param name="title">Stock title to search.</param>
    /// <returns>The <see cref="StockDto"/> if found.</returns>
    Task<StockDto> GetByTitle(string title);

    /// <summary>
    /// Gets all stocks.
    /// </summary>
    /// <returns>List of <see cref="StockDto"/>.</returns>
    Task<List<StockDto>> GetAll();

    /// <summary>
    /// Deletes a stock by identifier.
    /// </summary>
    /// <param name="id">Stock identifier.</param>
    /// <returns><c>true</c> if the stock was deleted; otherwise <c>false</c>.</returns>
    Task<bool> DeleteById(Guid id);
}
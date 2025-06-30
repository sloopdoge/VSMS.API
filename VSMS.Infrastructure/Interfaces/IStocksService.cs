using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces;

public interface IStocksService
{
    /// <summary>
    /// Creates a new stock entity with the supplied information and returns
    /// the created representation.
    /// </summary>
    /// <param name="stock">Data describing the stock to create.</param>
    /// <returns>The newly created <see cref="StockDto"/>.</returns>
    Task<StockDto> Create(StockDto stock);

    /// <summary>
    /// Updates an existing stock with the values provided in the model.
    /// </summary>
    /// <param name="stock">The updated stock information.</param>
    /// <returns>The modified <see cref="StockDto"/>.</returns>
    Task<StockDto> Update(StockDto stock);

    /// <summary>
    /// Retrieves details for the stock with the specified identifier.
    /// </summary>
    /// <param name="id">Unique identifier of the stock.</param>
    /// <returns>The matching <see cref="StockDto"/> if found.</returns>
    Task<StockDto> GetById(Guid id);

    /// <summary>
    /// Searches for a stock by its title.
    /// </summary>
    /// <param name="title">Title of the stock.</param>
    /// <returns>The <see cref="StockDto"/> when a match is found.</returns>
    Task<StockDto> GetByTitle(string title);

    /// <summary>
    /// Gets a collection of all stocks available in the system.
    /// </summary>
    /// <returns>List of <see cref="StockDto"/> objects.</returns>
    Task<List<StockDto>> GetAll();

    /// <summary>
    /// Removes the stock identified by the given id.
    /// </summary>
    /// <param name="id">Identifier of the stock to remove.</param>
    /// <returns><c>true</c> if the stock was deleted; otherwise <c>false</c>.</returns>
    Task<bool> DeleteById(Guid id);

    /// <summary>
    /// Retrieves all stocks that belong to the specified company.
    /// </summary>
    /// <returns>List of <see cref="StockDto"/>.</returns>
    Task<List<StockDto>> GetByCompanyId(Guid companyId);

    /// <summary>
    /// Gets the performance metrics for a particular stock.
    /// </summary>
    /// <param name="stockId">Identifier of the stock.</param>
    /// <returns>A <see cref="StockPerformanceDto"/> with calculated performance data.</returns>
    Task<StockPerformanceDto> GetStockPerformanceById(Guid stockId);

    /// <summary>
    /// Retrieves performance information for every stock in the system.
    /// </summary>
    /// <returns>List of <see cref="StockPerformanceDto"/> entries.</returns>
    Task<List<StockPerformanceDto>> GetAllStocksPerformance();

    /// <summary>
    /// Retrieves performance data for all stocks belonging to the specified company.
    /// </summary>
    /// <param name="companyId">Unique identifier of the company.</param>
    /// <returns>Collection of <see cref="StockPerformanceDto"/>.</returns>
    Task<List<StockPerformanceDto>> GetStocksPerformanceByCompanyId(Guid companyId);

    /// <summary>
    /// Returns historical stock values between the given dates.
    /// </summary>
    /// <param name="stockId">Identifier of the stock.</param>
    /// <param name="startDate">Start date of the history period.</param>
    /// <param name="endDate">End date of the history period.</param>
    /// <returns>List of <see cref="StockDto"/> entries or <c>null</c> if not found.</returns>
    Task<List<StockDto>?> GetHistoryById(Guid stockId, DateTime startDate, DateTime endDate);
}
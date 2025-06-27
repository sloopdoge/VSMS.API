using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Domain.Constants;
using VSMS.Domain.DTOs;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Application.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StocksController(
    ILogger<StocksController> logger,
    IStocksService stocksService) : ControllerBase
{
    /// <summary>
    /// Retrieves a stock by its identifier.
    /// </summary>
    /// <param name="stockId">Guid representation of the stock ID.</param>
    /// <returns>The <see cref="StockDto"/> model.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StockDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{stockId:guid}")]
    public async Task<IActionResult> GetStockById(Guid stockId)
    {
        try
        {
            var stock = await stocksService.GetById(stockId);
            return Ok(stock);
        }
        catch (StockNotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Retrieves all available stocks.
    /// </summary>
    /// <returns>List of <see cref="StockDto"/>.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StockDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAllStocks()
    {
        try
        {
            var stocks = await stocksService.GetAll();
            if (stocks is null || !stocks.Any())
                return NotFound();

            return Ok(stocks);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Creates a new stock.
    /// </summary>
    /// <param name="model">Stock model.</param>
    /// <returns>Created <see cref="StockDto"/>.</returns>
    [Authorize(Policy = PolicyNames.AdminOrCompanyAdmin)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StockDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> CreateStock([FromBody] StockDto model)
    {
        try
        {
            var stock = await stocksService.Create(model);
            return Ok(stock);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Updates an existing stock.
    /// </summary>
    /// <param name="stockId">Stock identifier.</param>
    /// <param name="model">Updated stock model.</param>
    /// <returns>Updated <see cref="StockDto"/>.</returns>
    [Authorize(Policy = PolicyNames.AdminOrCompanyAdmin)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StockDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("{stockId:guid}")]
    public async Task<IActionResult> UpdateStock(Guid stockId, [FromBody] StockDto model)
    {
        try
        {
            if (stockId == Guid.Empty)
                return BadRequest("Stock Id is empty.");

            var updated = await stocksService.Update(model);
            return Ok(updated);
        }
        catch (StockNotFoundException)
        {
            return NotFound($"Stock with ID: {stockId} not found.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Deletes a stock by its identifier.
    /// </summary>
    /// <param name="stockId">Stock identifier.</param>
    /// <returns>Indicates whether deletion succeeded.</returns>
    [Authorize(Policy = PolicyNames.AdminOrCompanyAdmin)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{stockId:guid}")]
    public async Task<IActionResult> DeleteStock(Guid stockId)
    {
        try
        {
            if (stockId == Guid.Empty)
                return BadRequest("Stock Id is empty.");

            var result = await stocksService.DeleteById(stockId);
            return result ? NoContent() : StatusCode(418);
        }
        catch (StockNotFoundException)
        {
            return NotFound($"Stock with ID: {stockId} not found.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }
}

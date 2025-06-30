using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Domain.Constants;
using VSMS.Domain.DTOs;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Interfaces;
using VSMS.Infrastructure.Identity;

namespace VSMS.Application.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StocksController(
    ILogger<StocksController> logger,
    IStocksService stocksService,
    IAuthorizationService authorizationService,
    ICompaniesService companiesService) : ControllerBase
{
    /// <summary>
    /// Gets detailed information about a specific stock.
    /// </summary>
    /// <param name="stockId">Guid representation of the stock ID.</param>
    /// <returns>The <see cref="StockDto"/> model.</returns>
    [Authorize]
    /// <summary>
    /// Retrieves performance data for all stocks in the system.
    /// </summary>
    /// <summary>
    /// Retrieves performance information for all stocks belonging to a specific company.
    /// </summary>
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
            if (stockId == Guid.Empty)
                return BadRequest("Incorrect input data");
            
            var stock = await stocksService.GetById(stockId);
            return Ok(stock);
        }
        catch (StockNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Retrieves a list of every stock currently stored in the system.
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
            return Ok(stocks);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Retrieves all stocks that belong to a particular company.
    /// </summary>
    /// <returns>List of <see cref="StockDto"/>.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StockDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("Company/{companyId:guid}")]
    public async Task<IActionResult> GetAllCompanyStocks(Guid companyId)
    {
        try
        {
            if(companyId == Guid.Empty)
                return BadRequest("Incorrect input data");
            
            var company = await companiesService.GetById(companyId);

            var stocks = await stocksService.GetByCompanyId(company.Id);
            if (!stocks.Any())
                return NotFound();

            return Ok(stocks);
        }
        catch (CompanyNotFoundException e)
        {
            logger.LogError(e.Message);
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Creates a new stock entry for a company.
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
            var authResult = await authorizationService.AuthorizeAsync(User,
                model.CompanyId ?? Guid.Empty, new CompanyOwnershipRequirement());
            if (!authResult.Succeeded)
                return Forbid();

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
    /// Updates an existing stock with new information.
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
    [HttpPut]
    public async Task<IActionResult> UpdateStock([FromBody] StockDto model)
    {
        try
        {
            var authResult = await authorizationService.AuthorizeAsync(User,
                model.CompanyId ?? Guid.Empty, new CompanyOwnershipRequirement());
            if (!authResult.Succeeded)
                return Forbid();

            var updated = await stocksService.Update(model);
            return Ok(updated);
        }
        catch (StockNotFoundException)
        {
            return NotFound($"Stock with ID: {model.Id} not found.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Deletes a stock with the given identifier.
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
                return BadRequest("Incorrect input data");

            var stock = await stocksService.GetById(stockId);
            var authResult = await authorizationService.AuthorizeAsync(User,
                stock.CompanyId ?? Guid.Empty, new CompanyOwnershipRequirement());
            if (!authResult.Succeeded)
                return Forbid();

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
    
    [Produces("application/json")]
    /// <summary>
    /// Retrieves performance statistics for a single stock.
    /// </summary>
    /// <param name="stockId">Identifier of the stock.</param>
    /// <returns>Performance metrics for the stock.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StockPerformanceDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("StocksPerformance/{stockId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetStockPerformanceById(Guid stockId)
    {
        try
        {
            if (stockId == Guid.Empty)
                return BadRequest("Incorrect input data");
            
            var stock = await stocksService.GetStockPerformanceById(stockId);

            return Ok(stock);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return StatusCode(500, e.Message);
        }
    }
    
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StockPerformanceDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("StocksPerformance")]
    [Authorize]
    public async Task<IActionResult> GetAllStocksPerformance()
    {
        try
        {
            var stocks = await stocksService.GetAllStocksPerformance();

            return Ok(stocks);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return StatusCode(500, e.Message);
        }
    }
    
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StockPerformanceDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("StocksPerformance/Company/{companyId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetAllCompanyStocksPerformance(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
                return BadRequest("Incorrect input data");
            
            var stocks = await stocksService.GetStocksPerformanceByCompanyId(companyId);

            return Ok(stocks);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return StatusCode(500, e.Message);
        }
    }
}

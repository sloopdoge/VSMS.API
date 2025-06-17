using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Domain.DTOs;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Application.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CompaniesController(
    ILogger<CompaniesController> logger,
    ICompaniesService companiesService) : ControllerBase
{

    /// <summary>
    /// Method to get Company by id.
    /// </summary>
    /// <param name="companyId">Guid representation of Company ID.</param>
    /// <returns>Model of Company.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{companyId:guid}")]
    public async Task<IActionResult> GetCompanyById(Guid companyId)
    {
        try
        {
            var company = await companiesService.GetById(companyId);
            return Ok(company);
        }
        catch (CompanyNotFoundException)
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
    /// Method to create Company.
    /// </summary>
    /// <param name="model">Company model.</param>
    /// <returns>Created Company model.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyDto model)
    {
        try
        {
            var company = await companiesService.Create(model);

            return Ok(company);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Method to update Company.
    /// </summary>
    /// <param name="companyId">Company ID.</param>
    /// <param name="model">Updated Company model.</param>
    /// <returns>Updated Company model.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("{companyId:guid}")]
    public async Task<IActionResult> UpdateCompany(Guid companyId, [FromBody] CompanyDto model)
    {
        try
        {
            if (companyId == Guid.Empty)
                return BadRequest("Company Id is empty.");
            
            var updatedCompany = await companiesService.Update(model);
            return Ok(updatedCompany);
        }
        catch (CompanyNotFoundException)
        {
            return NotFound($"Company with ID: {companyId} not found.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Method to Delete Company by ID.
    /// </summary>
    /// <param name="companyId">Company ID.</param>
    /// <returns>Indicates whether the deletion succeeded.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{companyId:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
                return BadRequest("Company Id is empty.");

            var result = await companiesService.DeleteById(companyId);

            return result
                ? NoContent()
                : StatusCode(418);
        }
        catch (CompanyNotFoundException)
        {
            return NotFound($"Company with ID: {companyId} not found.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

}

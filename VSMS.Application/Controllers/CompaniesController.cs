using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Domain.Constants;
using VSMS.Domain.DTOs;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Identity;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Application.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompaniesController(
    ILogger<CompaniesController> logger,
    ICompaniesService companiesService,
    ICompanyUsersService companyUsersService,
    IAuthorizationService authorizationService) : ControllerBase
{

    /// <summary>
    /// Retrieves details of a specific company using its identifier.
    /// </summary>
    /// <param name="companyId">Guid representation of Company ID.</param>
    /// <returns>The corresponding <see cref="CompanyDto"/>.</returns>
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
    /// Returns a list of all companies stored in the system.
    /// </summary>
    /// <returns>Collection of <see cref="CompanyDto"/> items.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CompanyDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        try
        {
            var company = await companiesService.GetAll();
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
    /// Retrieves all user profiles that are linked to the given company.
    /// </summary>
    /// <param name="companyId">Company identifier.</param>
    /// <returns>List of users in the company.</returns>
    [Authorize(Policy = PolicyNames.AdminOrCompanyAdminOrManager)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserProfileDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{companyId:guid}/users")]
    public async Task<IActionResult> GetAllUsersInCompany(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
                return BadRequest("Company Id is empty.");
            
            var authResult = await authorizationService.AuthorizeAsync(User, 
                companyId, new CompanyOwnershipRequirement());
            if (!authResult.Succeeded)
                return Forbid();

            var users = await companyUsersService.GetAllUsersInCompany(companyId);
            return Ok(users);
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
    /// Creates a new company using the provided data.
    /// </summary>
    /// <param name="model">Company model.</param>
    /// <returns>The created <see cref="CompanyDto"/>.</returns>
    [Authorize(Policy = PolicyNames.AdminOnly)]
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
    /// Updates the details of an existing company.
    /// </summary>
    /// <param name="companyId">Company ID.</param>
    /// <param name="model">Updated Company model.</param>
    /// <returns>The updated <see cref="CompanyDto"/>.</returns>
    [Authorize(Policy = PolicyNames.AdminOrCompanyAdmin)]
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
            
            var authResult = await authorizationService.AuthorizeAsync(User, 
                companyId, new CompanyOwnershipRequirement());
            if (!authResult.Succeeded)
                return Forbid();
            
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
    /// Deletes the specified company from the database.
    /// </summary>
    /// <param name="companyId">Company ID.</param>
    /// <returns>Indicates whether the deletion succeeded.</returns>
    [Authorize(Policy = PolicyNames.AdminOnly)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{companyId:guid}")]
    public async Task<IActionResult> DeleteCompanyById(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
                return BadRequest("Company Id is empty.");
            
            var authResult = await authorizationService.AuthorizeAsync(User, 
                companyId, new CompanyOwnershipRequirement());
            if (!authResult.Succeeded)
                return Forbid();

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

    /// <summary>
    /// Links a user account to the given company.
    /// </summary>
    /// <param name="companyId">Company identifier.</param>
    /// <param name="userId">User identifier.</param>
    /// <returns>Boolean result of the assignment.</returns>
    [Authorize(Policy = PolicyNames.AdminOrCompanyAdminOrManager)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("{companyId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> AssignUserToCompany(Guid companyId, Guid userId)
    {
        try
        {
            if (companyId == Guid.Empty || userId == Guid.Empty)
                return BadRequest("Company Id or User Id is empty.");
            
            var authResult = await authorizationService.AuthorizeAsync(User, 
                companyId, new CompanyOwnershipRequirement());
            if (!authResult.Succeeded)
                return Forbid();

            var result = await companyUsersService.AssignUserToCompany(userId, companyId);
            return Ok(result);
        }
        catch (UserNotFoundException)
        {
            return NotFound($"User with ID: {userId} not found.");
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
    /// Unassigns a user from the given company.
    /// </summary>
    /// <param name="companyId">Company identifier.</param>
    /// <param name="userId">User identifier.</param>
    /// <returns>Boolean indicating whether the unassignment succeeded.</returns>
    [Authorize(Policy = PolicyNames.AdminOrCompanyAdmin)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{companyId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> UnassignUserFromCompany(Guid companyId, Guid userId)
    {
        try
        {
            if (companyId == Guid.Empty || userId == Guid.Empty)
                return BadRequest("Company Id or User Id is empty.");
            
            var authResult = await authorizationService.AuthorizeAsync(User, 
                companyId, new CompanyOwnershipRequirement());
            if (!authResult.Succeeded)
                return Forbid();

            var result = await companyUsersService.UnassignUserFromCompany(userId, companyId);
            return Ok(result);
        }
        catch (UserNotFoundException)
        {
            return NotFound($"User with ID: {userId} not found.");
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

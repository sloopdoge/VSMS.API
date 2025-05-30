using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Identity.Domain.DTOs;
using VSMS.Identity.Domain.Models;
using VSMS.Identity.Infrastructure.Interfaces;

namespace VSMS.Identity.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    IUserService userService) : ControllerBase
{
    /// <summary>
    /// Method for login.
    /// </summary>
    /// <param name="model">Login model.</param>
    /// <returns>JWT token.</returns>
    [AllowAnonymous]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Token))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        try
        {
            if (model is null)
                return BadRequest($"Model is null.");

            if (string.IsNullOrEmpty(model.Email))
                return BadRequest($"Email is empty.");
            
            if (string.IsNullOrEmpty(model.Password))
                return BadRequest($"Password is empty.");
            
            var user = await userService.GetUserByEmail(model.Email);
            if (user is null)
                return NotFound($"User with email: {model.Email} not found.");
            
            var isPasswordCorrect = await userService.IsPasswordCorrect(user, model.Password);
            
            return isPasswordCorrect
                ? Ok() 
                : BadRequest($"Password is incorrect.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("Token/Validate")]
    public async Task<IActionResult> ValidateToken()
    {
        try
        {
            return NoContent();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }
}
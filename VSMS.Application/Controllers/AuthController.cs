using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Domain.DTOs;
using VSMS.Domain.Models;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    IUserService userService,
    ITokenService tokenService) : ControllerBase
{
    /// <summary>
    /// Authenticates the user with the provided credentials and
    /// returns a JWT token when successful.
    /// </summary>
    /// <param name="model">User credentials.</param>
    /// <returns>The result of the login attempt.</returns>
    [AllowAnonymous]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResultModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto model)
    {
        try
        {
            var loginResult = new LoginResultModel();
            
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                loginResult.Success = false;
                loginResult.Errors.Add("General", $"Incorrect input data");
                    
                return BadRequest(loginResult);
            }
            
            var user = await userService.GetUserByEmail(model.Email);
            if (user is null)
            {
                loginResult.Success = false;
                loginResult.Errors.Add(nameof(model.Email), $"User with email: {model.Email} - not found");
                return NotFound(loginResult);
            }
            
            var userProfile = await userService.GetUserProfileById(user.Id);
            if (userProfile is null)
            {
                loginResult.Success = false;
                loginResult.Errors.Add(nameof(model.Email), $"User Profile with email: {model.Email} - not found");
                return NotFound(loginResult);
            }

            var isPasswordCorrect = await userService.IsPasswordCorrect(user, model.Password);
            if (!isPasswordCorrect)
            {
                loginResult.Success = false;
                loginResult.Errors.Add(nameof(model.Password), $"Password is incorrect");
                return BadRequest(loginResult);
            }


            var token = await userService.GenerateToken(user, model.UseLongLivedToken);
            if (string.IsNullOrEmpty(token.Value))
            {
                loginResult.Success = false;
                loginResult.Errors.Add("General", $"There was error creating token");
                return BadRequest(loginResult);
            }

            loginResult.Success = true;
            loginResult.UserProfile = userProfile;
            loginResult.Token = token;
            loginResult.Errors.Clear();
            
            return Ok(loginResult);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Creates a new user account with the provided registration data.
    /// </summary>
    /// <param name="model">User registration information.</param>
    /// <returns>The result of the registration process.</returns>
    [AllowAnonymous]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResultModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
    {
        try
        {
            var user = await userService.GetUserByEmail(model.Email);
            if (user is not null)
                return BadRequest(new RegisterResultModel
                {
                    Success = false,
                    UserProfile = null,
                    Errors = [$"User with email: {model.Email} - already exist"]
                });

            var registerRes = await userService.RegisterUser(model);

            return registerRes.Success
                ? Ok(registerRes)
                : BadRequest(registerRes);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Validates the bearer token supplied in the request's Authorization header.
    /// </summary>
    /// <returns>200 OK with validation result, or an appropriate error code.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenValidationResultModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("Token/Validate")]
    public async Task<IActionResult> ValidateToken()
    {
        try
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized(new TokenValidationResultModel
                {
                    IsValid = false,
                    Error = "Authorization header is missing or malformed."
                });

            var token = authHeader["Bearer ".Length..].Trim();

            var result = tokenService.ValidateToken(token);

            if (!result.IsValid)
                return Unauthorized(result);

            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }
}
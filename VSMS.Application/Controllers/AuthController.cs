using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Domain.DTOs;
using VSMS.Domain.Models;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    IUserService userService) : ControllerBase
{
    /// <summary>
    /// Method for login.
    /// </summary>
    /// <param name="model">Login model.</param>
    /// <returns></returns>
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
            var user = await userService.GetUserByEmail(model.Email);
            if (user is null)
                return NotFound(
                    new LoginResultModel
                    {
                        Success = false,
                        UserProfile = null,
                        Token = null,
                        Errors = [$"User with email: {model.Email} - not found"]
                    });
            
            var isPasswordCorrect = await userService.IsPasswordCorrect(user, model.Password);
            if (!isPasswordCorrect)
                return BadRequest(new LoginResultModel
                {
                    Success = false,
                    UserProfile = null,
                    Token = null,
                    Errors = ["Password is incorrect"]
                });
            
            var token = await userService.GenerateToken(user, model.UseLongLivedToken);
            if (string.IsNullOrEmpty(token.Value))
                return BadRequest(new LoginResultModel
                {
                    Success = false,
                    UserProfile = null,
                    Token = null,
                    Errors = ["There was error creating token"]
                });
            
            return Ok(new LoginResultModel
            {
                Success = true,
                Token = token,
                UserProfile = await userService.GetUserProfileById(user.Id),
            });
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Method to register a user.
    /// </summary>
    /// <param name="model">Register model.</param>
    /// <returns></returns>
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
}
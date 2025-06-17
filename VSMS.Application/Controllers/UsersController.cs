using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Domain.DTOs;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Application.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController(
    ILogger<UsersController> logger,
    IUserService userService) : ControllerBase
{
    /// <summary>
    /// Method to get all User Profiles.
    /// </summary>
    /// <returns>List with models of User Profiles.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserProfileDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAllUserProfiles()
    {
        try
        {
            var users = await userService.GetAllUserProfiles();
            if (users is null || !users.Any())
                return NotFound();
            
            return Ok(users);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Method to get User Profile by id.
    /// </summary>
    /// <param name="userId">Guid representation of User ID.</param>
    /// <returns>Model of User Profile.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProfileById(Guid userId)
    {
        try
        {
            var user = await userService.GetUserProfileById(userId);
            return Ok(user);
        }
        catch (UserNotFoundException)
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
    /// Method to create User.
    /// </summary>
    /// <param name="model">User Create model.</param>
    /// <returns>User Profile of Created User.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto model)
    {
        try
        {
            var user = await userService.CreateUser(model);
            return Ok(user);
        }
        catch (UserNotFoundException)
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
    /// Method to update User.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="model">Updated User Profile model.</param>
    /// <returns>Updated User Profile.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserProfileDto model)
    {
        try
        {
            if (userId == Guid.Empty)
                return BadRequest("User Id is empty.");

            var updatedUser = await userService.UpdateUserProfile(model);
            return Ok(updatedUser);
        }
        catch (UserNotFoundException)
        {
            return NotFound($"User with ID: {userId} not found.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// Method to Delete User by ID.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns>Indicates whether the deletion succeeded.</returns>
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
                return BadRequest("User Id is empty.");

            var result = await userService.DeleteUserById(userId);

            return result
                ? NoContent()
                : StatusCode(418);
        }
        catch (UserNotFoundException)
        {
            return NotFound($"User with ID: {userId} not found.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }
}
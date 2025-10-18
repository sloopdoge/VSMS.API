using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Domain.Exceptions;

namespace VSMS.Infrastructure.Interfaces;

public interface ICompanyUsersService
{
    /// <summary>
    /// Links an existing user account to the provided company.
    /// </summary>
    /// <param name="userId">Identifier of the user to assign.</param>
    /// <param name="companyId">Identifier of the company that will receive the user.</param>
    /// <returns><c>true</c> when the user was successfully assigned.</returns>
    Task<bool> AssignUserToCompany(Guid userId, Guid companyId);

    /// <summary>
    /// Detaches a user from the given company.
    /// </summary>
    /// <param name="userId">Identifier of the user to remove.</param>
    /// <param name="companyId">Identifier of the company.</param>
    /// <returns><c>true</c> if the user was successfully unassigned.</returns>
    Task<bool> UnassignUserFromCompany(Guid userId, Guid companyId);

    /// <summary>
    /// Retrieves all users currently assigned to the specified company.
    /// </summary>
    /// <param name="companyId">Identifier of the company.</param>
    /// <returns>A collection of <see cref="UserProfileDto"/> representing the company's users.</returns>
    Task<List<UserProfileDto>> GetAllUsersInCompany(Guid companyId);

    /// <summary>
    /// Retrieves the company that contains the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose associated company is to be retrieved.</param>
    /// <returns>
    /// A <see cref="Company"/> entity that the user belongs to.
    /// </returns>
    /// <exception cref="UserCompanyNotFoundException">
    /// Thrown when no company containing the specified user is found.
    /// </exception>
    /// <remarks>
    /// This method performs a query against the <c>Companies</c> repository to find the company
    /// that includes a user with the given <paramref name="userId"/>. 
    /// It is optimized to avoid unnecessary data loading and should be used when you need to
    /// determine the user's company context.
    /// </remarks>
    Task<Company?> GetUserCompany(Guid userId);
}
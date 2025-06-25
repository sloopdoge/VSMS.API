using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces;

public interface ICompanyUsersService
{
    /// <summary>
    /// Assigns a user to the specified company.
    /// </summary>
    /// <param name="userId">Identifier of the user to assign.</param>
    /// <param name="companyId">Identifier of the company.</param>
    /// <returns><c>true</c> if the user was successfully assigned.</returns>
    Task<bool> AssignUserToCompany(Guid userId, Guid companyId);

    /// <summary>
    /// Removes a user from the specified company.
    /// </summary>
    /// <param name="userId">Identifier of the user to remove.</param>
    /// <param name="companyId">Identifier of the company.</param>
    /// <returns><c>true</c> if the user was unassigned.</returns>
    Task<bool> UnassignUserFromCompany(Guid userId, Guid companyId);

    /// <summary>
    /// Gets all users assigned to a company.
    /// </summary>
    /// <param name="companyId">Identifier of the company.</param>
    /// <returns>List of <see cref="UserProfileDto"/> in the company.</returns>
    Task<List<UserProfileDto>> GetAllUsersInCompany(Guid companyId);
}
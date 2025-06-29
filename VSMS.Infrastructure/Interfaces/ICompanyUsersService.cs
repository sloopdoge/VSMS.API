using VSMS.Domain.DTOs;

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
}